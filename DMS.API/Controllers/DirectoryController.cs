using AutoMapper;
using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DocumentManagementSystem.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectoryController : ControllerBase
    {
        private readonly IUnitOfWork _uOW;
        private readonly IMapper _mapper;

        public DirectoryController(IUnitOfWork UOW, IMapper mapper) 
        {
            _uOW = UOW;
            _mapper = mapper;
        }

        [HttpGet("get-all-directories")]

        public async Task<IActionResult> Get()
        {
            var allDirectories = await _uOW.directoryRepository.GetAllAsync();
            var directories = _mapper.Map<List<MyDirectoryDto>>(allDirectories);
            if (directories is not null)
            {
                return Ok(directories);
            }
            return BadRequest();
        }

        [HttpGet("get-directory-by-id/{id}")]
        
        public async Task<IActionResult> Get(int id)
        {
            var directory = await _uOW.directoryRepository.GetAsync(id);
            var directoryDto = _mapper.Map<MyDirectoryDto>(directory);
            if (directoryDto is not null)
            {
                return Ok(directoryDto);
            }
            return BadRequest($"This id [{id}] was Not Found");
        }

        [HttpGet("get-directories-in-workspace")]

        public async Task<IActionResult> DirectoriesInWorkspace(int workspaceId)
        {
            if (_uOW.workspaceRepository.workspaceExists(workspaceId))
            {
                var directories = await _uOW.directoryRepository.GetDirectoriesInWorkspace(workspaceId);
                var directoriesMap = _mapper.Map<List<MyDirectoryDto>>(directories);
                if (directoriesMap is not null)
                {
                    return Ok(directoriesMap);
                }
                return BadRequest("Something went wrong");
            }
            return BadRequest("Workspace doesn't exist");
        }

        [HttpPost("add-new-directory")]

        public async Task<ActionResult> Post(MyDirectoryDto directoryDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_uOW.workspaceRepository.workspaceExists(directoryDto.WorkspaceId))
                    {
                        var directoryMap = _mapper.Map<MyDirectory>(directoryDto);
                        await _uOW.directoryRepository.AddAsync(directoryMap);
                        return Ok(directoryDto);
                    }
                    return NotFound("Workspace Was Not Found");
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-existing-directory")]

        public async Task<ActionResult> Put(MyDirectoryDto directoryDto)
        {
            try
            {
                if (directoryDto is null)
                    return BadRequest(ModelState);
                if (ModelState.IsValid)
                {
                    if (_uOW.directoryRepository.directoryExists(directoryDto.id))
                    {
                        var directoryMap = _mapper.Map<MyDirectory>(directoryDto);
                        await _uOW.directoryRepository.UpdateAsync(directoryMap);
                        return Ok(directoryDto);
                    }
                    return BadRequest($"Directory Not Found, Id [{directoryDto.id}] is Incorrect");
                }
                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete-directory-by-id/{id}")]

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_uOW.directoryRepository.directoryExists(id))
                    {
                        await _uOW.directoryRepository.DeleteAsync(id);
                        return Ok("Directory was deleted!");
                    }
                    return BadRequest($"Directory Not Found, Id [{id}] is Incorrect");
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
