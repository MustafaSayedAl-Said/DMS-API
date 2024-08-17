using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories
{
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        private readonly DataContext _context;
        public DocumentRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public bool documentExists(int id)
        {
            return _context.Documents.Any(d => d.Id == id);
        }

        public async Task<ICollection<Document>> GetDocumentsInDirectory(int directoryId)
        {
            var documents = await _context.Documents.AsNoTracking().Where(d => d.DirectoryId == directoryId).ToListAsync();

            return documents;
        }
    }
}
