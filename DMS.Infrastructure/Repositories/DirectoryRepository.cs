using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Core.Sharing;
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

        public async Task<(IEnumerable<MyDirectory>, int TotalCount)> GetAllAsync(DirectoryParams directoryParams)
        {
            List<MyDirectory> query;

            //search by WorkspaceId
            if(directoryParams.WorkspaceId.HasValue)
            {
                query = await _context.Directories.AsNoTracking().Where(d => d.WorkspaceId == directoryParams.WorkspaceId).ToListAsync();
            }
            else
            {
                query = await _context.Directories.AsNoTracking().ToListAsync();
            }

            //search by name
            if(!string.IsNullOrEmpty(directoryParams.Search))
                query = query.Where(x => x.Name.ToLower().Contains(directoryParams.Search.ToLower())).ToList();

            int totalCount = query.Count;

            //sorting
            if (!string.IsNullOrEmpty(directoryParams.Sort))
            {
                query = directoryParams.Sort switch
                {
                    "NameAsc" => query.OrderBy(x => x.Name).ToList(),
                    "NameDesc" => query.OrderByDescending(x => x.Name).ToList(),
                    _ => query.OrderBy(x => x.Name).ToList(),
                };
            }

            //paging
            query = query.Skip((directoryParams.PageSize) * (directoryParams.PageNumber - 1)).Take(directoryParams.PageSize).ToList();

            return (query, totalCount);
        }

        //public async Task<ICollection<MyDirectory>> GetDirectoriesInWorkspace(int workspaceId)
        //{
        //    var directories = await _context.Directories.AsNoTracking().Where(d => d.WorkspaceId == workspaceId).ToListAsync();

        //    return directories;
        //}
    }
}
