using DMS.Core.Entities;

namespace DMS.Core.Interfaces
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        public bool documentExists(int id);

        public Task<ICollection<Document>> GetDocumentsInDirectory(int directoryId);
    }
}
