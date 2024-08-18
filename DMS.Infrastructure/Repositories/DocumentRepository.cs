using AutoMapper;
using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Infrastructure.Data;
using DMS.Infrastructure.Data.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace DMS.Infrastructure.Repositories
{
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        private readonly DataContext _context;
        private readonly IFileProvider _fileProvider;
        private readonly IMapper _mapper;

        public DocumentRepository(DataContext context, IFileProvider fileProvider, IMapper mapper) : base(context)
        {
            _context = context;
            _fileProvider = fileProvider;
            _mapper = mapper;
        }

        public async Task<bool> AddSync(DocumentDto dto)
        {
            if (dto.DocumentContent is not null)
            {
                var root = "/documents";
                var documentName = $"{Guid.NewGuid()}" + dto.DocumentContent.FileName;
                if (!Directory.Exists("wwwroot" + root))
                {
                    Directory.CreateDirectory("wwwroot" + root);
                }
                var src = root + documentName;
                var docInfo = _fileProvider.GetFileInfo(src);
                var rootPath = docInfo.PhysicalPath;

                using (var fileStream = new FileStream(rootPath, FileMode.Create))
                {
                    await dto.DocumentContent.CopyToAsync(fileStream);
                }

                //Create New Document
                var documentMap = _mapper.Map<Document>(dto);
                documentMap.Name = dto.DocumentContent.FileName;
                documentMap.DocumentContent = src;
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
    }
}
