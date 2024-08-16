﻿using AutoMapper;
using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DocumentManagementSystem.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet("get-all-workspaces")]

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

        [HttpGet("get-workspace-by-id/{id}")]

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

        [HttpPost("add-new-workspace")]
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

        [HttpPut("update-existing-workspace")]

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

        [HttpDelete("delete-workspace-by-id/{id}")]

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
