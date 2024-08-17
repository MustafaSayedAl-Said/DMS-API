using AutoMapper;
using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DocumentManagementSystem.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    [Route("api/workspaces")]
    [ApiController]
    public class WorkspacesController : ControllerBase
    {
        private readonly IUnitOfWork _uOW;
        private readonly IMapper _mapper;
        public WorkspacesController(IUnitOfWork UOW, IMapper mapper)
        {
            _uOW = UOW;
            _mapper = mapper;
        }

        [HttpGet]

        public async Task<IActionResult> Get()
        {
            var allWorkspaces = await _uOW.workspaceRepository.GetAllAsync();
            var workspaces = _mapper.Map<List<WorkspaceDto>>(allWorkspaces);
            if (allWorkspaces is not null)
            {
                return Ok(workspaces);
            }
            return BadRequest("Not Found");
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> Get(int id)
        {
            var workspace = await _uOW.workspaceRepository.GetAsync(id);
            var workspaceDto = _mapper.Map<WorkspaceDto>(workspace);
            if (workspace is not null)
            {
                return Ok(workspaceDto);
            }

            return BadRequest($"This id [{id}] was Not Found");
        }

        [HttpGet("user/{userId}")]

        public async Task<IActionResult> GetByUser(int userId)
        {
            if (_uOW.userRepository.userExists(userId))
            {
                var workspace = await _uOW.workspaceRepository.getWorkspaceByUserId(userId);
                var workspaceDto = _mapper.Map<WorkspaceDto>(workspace);
                if (workspace is not null)
                {
                    return Ok(workspaceDto);
                }

                return BadRequest("Something went wrong");
            }
            return BadRequest($"This User id [{userId}] was Not Found");
        }

        [HttpPost]
        public async Task<ActionResult> Post(WorkspaceDto workspaceDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var workspaceMap = _mapper.Map<Workspace>(workspaceDto);
                    await _uOW.workspaceRepository.AddAsync(workspaceMap);
                    return Ok(workspaceDto);
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpPut]

        public async Task<ActionResult> Put(WorkspaceDto workspaceDto)
        {
            try
            {
                if (workspaceDto == null)
                    return BadRequest(ModelState);
                if (ModelState.IsValid)
                {
                    if (_uOW.workspaceRepository.workspaceExists(workspaceDto.id))
                    {
                        var workspaceMap = _mapper.Map<Workspace>(workspaceDto);
                        await _uOW.workspaceRepository.UpdateAsync(workspaceMap);
                        return Ok(workspaceDto);
                    }
                    return BadRequest($"Workspace Not Found, Id [{workspaceDto.id}] is Incorrect");
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
                    if (_uOW.workspaceRepository.workspaceExists(id))
                    {
                        var userId = _uOW.workspaceRepository.getUserId(id);
                        await _uOW.workspaceRepository.DeleteAsync(id);
                        await _uOW.userRepository.DeleteAsync(userId);
                        return Ok("Workspace was deleted!");
                    }
                    return BadRequest($"Workspace Not Found, Id [{id}] Incorrect");
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
