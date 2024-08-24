using Azure;
using DMS.Core.Dto;
using DMS.Core.Sharing;
using Microsoft.AspNetCore.JsonPatch;

namespace DMS.Services.Interfaces
{
    public interface IDirectoryService
    {
        Task<(List<MyDirectoryDto>, int)> GetAllDirectoriesAsync(DirectoryParams directoryParams);

        Task<MyDirectoryDto> GetDirectoryByIdAsync(int id);

        Task<bool> AddDirectoryAsync(MyDirectoryDto directoryDto);

        Task<bool> UpdateDirectoryAsync(MyDirectoryDto directoryDto);

        Task<bool> SoftDeleteDirectoryAsync(int id);

        public Task<bool> VerifyWorkspaceOwnershipAsync(int workspaceId, int userId);

        public Task<bool> VerifyDirectoryOwnershipAsync(int directoryId, int userId);

        public Task<bool> UpdateDirectoryNameAsync(int id, string newName);

    }
}
