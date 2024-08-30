﻿using DMS.API.Errors;
using DMS.Core.Dto;
using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userDto = await _userService.UserLogin(loginDto);
                    return Ok(userDto);
                }
                return BadRequest(new BaseCommonResponse(500));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseCommonResponse(401, ex.Message));
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userDto = await _userService.UserRegister(registerDto);
                    return Ok(userDto);
                }
                return BadRequest("Something went Wrong");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("test")]

        public ActionResult<string> Test()
        {
            return "hi";
        }


        [Authorize]
        [HttpGet("current")]

        public async Task<IActionResult> Get()
        {
            try
            {
                var userDto = await _userService.GetCurrentUser(HttpContext);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("check")]

        public async Task<IActionResult> CheckEmailExist([FromQuery] string email)
        {
            var res = await _userService.CheckEmailExistance(email);
            return Ok(res);
        }

        [Authorize]
        [HttpGet("workspace")]
        public async Task<IActionResult> GetUserWorkspace()
        {
            try
            {
                var res = await _userService.GetUserWorkspace(HttpContext);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var res = await _userService.GetAllUsersAsync();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}