using AutoMapper;
using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Core.Sharing;
using DMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;

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

        public async Task<bool> AddSync(DocumentDto dto, string name)
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
                documentMap.OwnerName = name;
                documentMap.ModifyDate = DateTime.Now.Date;
                await _context.Documents.AddAsync(documentMap);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<(IEnumerable<Document>, int TotalCount)> GetAllAsync(DocumentParams documentParams)
        {
            List<Document> query;

            //search by directoryId
            query = await _context.Documents.AsNoTracking().Where(d => d.DirectoryId == documentParams.DirectoryId && d.IsDeleted == false).ToListAsync();
            //if (documentParams.DirectoryId.HasValue)
            //{

            //}
            //else
            //{
            //    query = await _context.Documents.AsNoTracking().ToListAsync();
            //}

            //search by name
            if (!string.IsNullOrEmpty(documentParams.Search))
                query = query.Where(x => x.Name.ToLower().Contains(documentParams.Search.ToLower())).ToList();

            int totalCount = query.Count();

            //sorting
            if (!string.IsNullOrEmpty(documentParams.Sort))
            {
                query = documentParams.Sort switch
                {
                    "NameAsc" => query.OrderBy(x => x.Name).ToList(),
                    "NameDesc" => query.OrderByDescending(x => x.Name).ToList(),
                    _ => query.OrderBy(x => x.Name).ToList(),
                };
            }

            //paging
            query = query.Skip((documentParams.PageSize) * (documentParams.PageNumber - 1)).Take(documentParams.PageSize).ToList();

            return (query, totalCount);
        }

        public bool documentExists(int id)
        {
            return _context.Documents.Any(d => d.Id == id);
        }

        //public async Task<ICollection<Document>> GetDocumentsInDirectory(int directoryId)
        //{
        //    var documents = await _context.Documents.AsNoTracking().Where(d => d.DirectoryId == directoryId).ToListAsync();

        //    return documents;
        //}

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

        public async Task<bool> SoftDeleteDocumentAsync(int id)
        {
            var document = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
            {
                throw new Exception("Something Went Wrong");
            }

            document.IsDeleted = true;

            _context.Documents.Update(document);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateDocumentVisibilityAsync(int id)
        {
            var document = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
            {
                throw new Exception("Something went wrong");
            }

            document.IsPublic = true;
            _context.Documents.Update(document);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
