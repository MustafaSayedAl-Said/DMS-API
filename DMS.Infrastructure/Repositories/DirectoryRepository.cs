using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories
{
    public class DirectoryRepository : GenericRepository<MyDirectory>, IDirectoryRepository
    {
        private readonly DataContext _context;
        public DirectoryRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public bool directoryExists(int id)
        {
            return _context.Directories.Any(d => d.Id == id);
        }

        public async Task<ICollection<MyDirectory>> GetDirectoriesInWorkspace(int workspaceId)
        {
            var directories = await _context.Directories.AsNoTracking().Where(d => d.WorkspaceId == workspaceId).ToListAsync();

            return directories;
        }
    }
}
