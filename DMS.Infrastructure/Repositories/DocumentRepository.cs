using AutoMapper;
using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DMS.Infrastructure.Repositories
{
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public DocumentRepository(DataContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> AddSync(DocumentDto dto)
        {
            if (dto.DocumentContent is not null)
            {
                var root = "/documents/";
                var documentName = $"{Guid.NewGuid()}{Path.GetExtension(dto.DocumentContent.FileName)}";
                var directoryPath = Path.Combine("wwwroot", root.TrimStart('/'));

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var src = Path.Combine(root, documentName);
                var fullFilePath = Path.Combine(directoryPath, documentName);

                using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await dto.DocumentContent.CopyToAsync(fileStream);
                }

                // Map and save the document
                var documentMap = _mapper.Map<Document>(dto);
                documentMap.Name = dto.DocumentContent.FileName;
                documentMap.DocumentContent = src; // Relative path, as stored in the database
                await _context.Documents.AddAsync(documentMap);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
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

        public async Task<bool> UpdateAsync(DocumentDto dto)
        {
            if (dto.DocumentContent is not null)
            {
                var root = "/documents/";
                var documentName = $"{Guid.NewGuid()}{Path.GetExtension(dto.DocumentContent.FileName)}";
                var directoryPath = Path.Combine("wwwroot", root.TrimStart('/'));

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var src = Path.Combine(root, documentName);
                var fullFilePath = Path.Combine(directoryPath, documentName);

                using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await dto.DocumentContent.CopyToAsync(fileStream);
                }

                // Map and save the document
                var documentMap = _mapper.Map<Document>(dto);
                documentMap.Name = dto.DocumentContent.FileName;
                documentMap.DocumentContent = src; // Relative path, as stored in the database
                _context.Documents.Update(documentMap);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
