using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Infrastructure.Data;

namespace DMS.Infrastructure.Repositories
{
    public class DirectoryRepository : GenericRepository<MyDirectory>, IDirectoryRepository
    {
        public DirectoryRepository(DataContext context) : base(context)
        {
        }
    }
}
