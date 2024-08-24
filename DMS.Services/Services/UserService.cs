using AutoMapper;
using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Services.Extensions;
using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Services.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<UserDto> UserLogin(LoginDto loginDto)
        {
            if (loginDto == null)
                throw new ArgumentNullException(nameof(loginDto));

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user is null)
                throw new Exception("Email Doesn't exist");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                throw new Exception("Password is wrong");

            return new UserDto
            {
                DisplayName = user.DisplayName,
                Email = loginDto.Email,
                Token = _tokenService.CreateToken(user),
                Id = user.Id
            };
        }

        public async Task<UserDto> UserRegister(RegisterDto registerDto)
        {
            if (registerDto == null)
                throw new ArgumentNullException(nameof(registerDto));
            if (CheckEmailExistance(registerDto.Email).Result)
            {
                throw new Exception("Email already exists");
            }
            var user = new User
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                Workspace = new Workspace
                {
                    Name = registerDto.WorkspaceName,
                }
            };
            var result = await _userManager.CreateAsync(user, registerDto.Paswword);
            if (result.Succeeded == false)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                throw new Exception(string.Join("\n", errors));
            }

            var CreatedUser = await _userManager.FindByEmailAsync(registerDto.Email);

            if (CreatedUser is null)
                throw new Exception("Something Went Wrong");

                return new UserDto
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                Token = _tokenService.CreateToken(CreatedUser),
                Id = CreatedUser.Id
            };
        }

        public async Task<UserDto> GetCurrentUser(HttpContext httpContext)
        {
            //var email = httpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

            //if (email == null)
            //{
            //    throw new Exception("User is not authenticated");
            //}

            var user = await _userManager.FindEmailByClaim(httpContext.User);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                Id = user.Id
            };
        }

        public async Task<bool> CheckEmailExistance(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        public async Task<WorkspaceDto> GetUserWorkspace(HttpContext httpContext)
        {
            //var email = httpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            //if (email == null)
            //{
            //    throw new Exception("User is not authenticated");
            //}
            //var user = await _userManager.Users.Include(x => x.Workspace).SingleOrDefaultAsync(x => x.Email == email);
            var user = await _userManager.FindUserByClaimWithWorkspace(httpContext.User);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var _result = _mapper.Map<WorkspaceDto>(user.Workspace);

            return _result;

        }
    }
}
