using DMS.Core.Entities;
using DMS.Core.Sharing;

namespace DMS.Core.Interfaces
{
    public interface IDirectoryRepository : IGenericRepository<MyDirectory>
    {
        public bool directoryExists(int id);

        public Task<IEnumerable<MyDirectory>> GetAllAsync(DirectoryParams directoryParams);

        //public Task<ICollection<MyDirectory>> GetDirectoriesInWorkspace(int workspaceId);
    }
}
