using DMS.Core.Entities;

namespace DMS.Core.Interfaces
{
    public interface IDirectoryRepository : IGenericRepository<MyDirectory>
    {
        public bool directoryExists(int id);

        public Task<ICollection<MyDirectory>> GetDirectoriesInWorkspace(int workspaceId);
    }
}
