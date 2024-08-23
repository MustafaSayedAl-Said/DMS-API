using AutoMapper;
using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Core.Sharing;
using DMS.Services.Interfaces;

namespace DMS.Services.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly IUnitOfWork _uOW;
        private readonly IMapper _mapper;

        public DirectoryService(IUnitOfWork UOW, IMapper mapper)
        {
            _uOW = UOW;
            _mapper = mapper;
        }
        public async Task<bool> AddDirectoryAsync(MyDirectoryDto directoryDto)
        {
            if (_uOW.workspaceRepository.workspaceExists(directoryDto.WorkspaceId))
            {
                var directoryMap = _mapper.Map<MyDirectory>(directoryDto);
                await _uOW.directoryRepository.AddAsync(directoryMap);
                return true;
            }
            throw new Exception("Workspace Was Not Found");
        }

        public async Task<bool> DeleteDirectoryAsync(int id)
        {
            if (_uOW.directoryRepository.directoryExists(id))
            {
                await _uOW.directoryRepository.DeleteAsync(id);
                return true;
            }

            throw new Exception($"Directory Not Found, Id[{id}] is Incorrect");
        }

        public async Task<(List<MyDirectoryDto>, int)> GetAllDirectoriesAsync(DirectoryParams directoryParams)
        {
            if (_uOW.workspaceRepository.workspaceExists(directoryParams.WorkspaceId))
            {
                var (allDirectories, totalItems) = await _uOW.directoryRepository.GetAllAsync(directoryParams);
                var directories = _mapper.Map<List<MyDirectoryDto>>(allDirectories);
                return (directories, totalItems);
            }
            throw new Exception("Workspace doesn't exist");
        }

        public async Task<MyDirectoryDto> GetDirectoryByIdAsync(int id)
        {
            var directory = await _uOW.directoryRepository.GetAsync(id);
            if (directory == null)
                throw new Exception($"This id [{id}] was Not Found");

            return _mapper.Map<MyDirectoryDto>(directory);
        }

        public async Task<bool> UpdateDirectoryAsync(MyDirectoryDto directoryDto)
        {
            if (directoryDto == null)
                throw new ArgumentNullException(nameof(directoryDto));
            if (_uOW.directoryRepository.directoryExists(directoryDto.id))
            {
                var directoryMap = _mapper.Map<MyDirectory>(directoryDto);
                await _uOW.directoryRepository.UpdateAsync(directoryMap);
                return true;
            }
            throw new Exception($"Directory Not Found, ID [{directoryDto.id}] is Incorrect");
        }
    }
}
