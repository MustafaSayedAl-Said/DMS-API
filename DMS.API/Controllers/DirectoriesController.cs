using AutoMapper;
using DMS.API.Helper;
using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Core.Sharing;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    [Route("api/directories")]
    [ApiController]
    public class DirectoriesController : ControllerBase
    {
        private readonly IUnitOfWork _uOW;
        private readonly IMapper _mapper;

        public DirectoriesController(IUnitOfWork UOW, IMapper mapper)
        {
            _uOW = UOW;
            _mapper = mapper;
        }

        [HttpGet]

        public async Task<IActionResult> Get([FromQuery]DirectoryParams directoryParams)
        {
            if (_uOW.workspaceRepository.workspaceExists(directoryParams.WorkspaceId))
            {
                var (allDirectories, totalItems) = await _uOW.directoryRepository.GetAllAsync(directoryParams);
                var directories = _mapper.Map<List<MyDirectoryDto>>(allDirectories);
                if (directories is not null)
                {
                    return Ok(new Pagination<MyDirectoryDto>(totalItems, directoryParams.PageSize, directoryParams.PageNumber, directories));
                }
                return BadRequest();
            }
            return BadRequest("Workspace doesn't exist");

        }

        [HttpGet("{id}")]

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

        //[HttpGet("workspace/{workspaceId}")]

        //public async Task<IActionResult> DirectoriesInWorkspace(int workspaceId)
        //{
        //    if (_uOW.workspaceRepository.workspaceExists(workspaceId))
        //    {
        //        var directories = await _uOW.directoryRepository.GetDirectoriesInWorkspace(workspaceId);
        //        var directoriesMap = _mapper.Map<List<MyDirectoryDto>>(directories);
        //        if (directoriesMap is not null)
        //        {
        //            return Ok(directoriesMap);
        //        }
        //        return BadRequest("Something went wrong");
        //    }
        //    return BadRequest("Workspace doesn't exist");
        //}

        [HttpPost]

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

        [HttpPut]

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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]

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
