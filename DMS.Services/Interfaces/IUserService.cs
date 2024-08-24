using DMS.Core.Dto;
using DMS.Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Services.Interfaces
{
    public interface IUserService
    {
        public Task<UserDto> UserLogin(LoginDto loginDto);

        public Task<UserDto> UserRegister(RegisterDto registerDto);

        public Task<UserDto> GetCurrentUser(HttpContext httpContext);

        public Task<bool> CheckEmailExistance(string email);

        public Task<WorkspaceDto> GetUserWorkspace(HttpContext httpContext);
    }
}
