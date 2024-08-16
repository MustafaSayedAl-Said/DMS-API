using DMS.Core.Interfaces;
using DMS.Infrastructure.Data;

namespace DMS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        public IWorkspaceRepository workspaceRepository { get; }

        public IDirectoryRepository directoryRepository { get; }

        public IUserRepository userRepository { get; }

        public IDocumentRepository documentRepository { get; }

        public UnitOfWork(DataContext context)
        {
            _context = context;
            workspaceRepository = new WorkspaceRepository(_context);
            directoryRepository = new DirectoryRepository(_context);
            userRepository = new UserRepository(_context);
            documentRepository = new DocumentRepository(_context);
        }
    }
}
