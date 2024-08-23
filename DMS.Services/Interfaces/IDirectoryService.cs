using DMS.Core.Dto;
using DMS.Core.Sharing;

namespace DMS.Services.Interfaces
{
    public interface IDirectoryService
    {
        Task<(List<MyDirectoryDto>, int)> GetAllDirectoriesAsync(DirectoryParams directoryParams);

        Task<MyDirectoryDto> GetDirectoryByIdAsync(int id);

        Task<bool> AddDirectoryAsync(MyDirectoryDto directoryDto);

        Task<bool> UpdateDirectoryAsync(MyDirectoryDto directoryDto);

        Task<bool> DeleteDirectoryAsync(int id);
    }
}
