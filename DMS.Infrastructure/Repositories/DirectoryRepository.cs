using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Infrastructure.Repositories
{
    public class DirectoryRepository : GenericRepository<MyDirectory>, IDirectoryRepository
    {
        public DirectoryRepository(DataContext context) : base(context)
        {
        }
    }
}
