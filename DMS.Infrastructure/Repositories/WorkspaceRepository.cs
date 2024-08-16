using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Infrastructure.Data;

namespace DMS.Infrastructure.Repositories
{
    public class WorkspaceRepository : GenericRepository<Workspace>, IWorkspaceRepository
    {
        private readonly DataContext _context;
        public WorkspaceRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public int getUserId(int id)
        {
            var workspace = _context.Workspaces.Find(id);
            var userId = workspace.UserId;
            return userId;
        }

        public bool workspaceExists(int id)
        {
            return _context.Workspaces.Any(w => w.Id == id);
        }
    }
}
