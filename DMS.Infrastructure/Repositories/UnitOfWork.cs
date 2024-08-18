using AutoMapper;
using DMS.Core.Interfaces;
using DMS.Infrastructure.Data;
using Microsoft.Extensions.FileProviders;

namespace DMS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IFileProvider _fileProvider;


        public IWorkspaceRepository workspaceRepository { get; }

        public IDirectoryRepository directoryRepository { get; }

        public IUserRepository userRepository { get; }

        public IDocumentRepository documentRepository { get; }

        public UnitOfWork(DataContext context, IFileProvider fileProvider, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _fileProvider = fileProvider;
            workspaceRepository = new WorkspaceRepository(_context);
            directoryRepository = new DirectoryRepository(_context);
            userRepository = new UserRepository(_context);
            documentRepository = new DocumentRepository(_context, _fileProvider, _mapper);
        }
    }
}
