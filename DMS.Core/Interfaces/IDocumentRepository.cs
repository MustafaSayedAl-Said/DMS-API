using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Sharing;

namespace DMS.Core.Interfaces
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        public Task<IEnumerable<Document>> GetAllAsync(DocumentParams documentParams);
        public bool documentExists(int id);

        public Task<ICollection<Document>> GetDocumentsInDirectory(int directoryId);

        Task<bool> AddSync(DocumentDto dto);

        Task<bool> UpdateAsync(DocumentDto dto);
    }
}
