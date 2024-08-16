using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Core.Interfaces
{
    public interface IUnitOfWork
    {
        public IWorkspaceRepository workspaceRepository { get; }

        public IDirectoryRepository directoryRepository { get; }

        public IUserRepository userRepository { get; }

        public IDocumentRepository documentRepository { get; }
    }
}
