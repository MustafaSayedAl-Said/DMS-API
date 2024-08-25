using DMS.Core.Dto;
using Microsoft.AspNetCore.Http;

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
