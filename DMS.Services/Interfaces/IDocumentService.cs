using DMS.Core.Dto;
using DMS.Core.Sharing;

namespace DMS.Services.Interfaces
{
    public interface IDocumentService
    {
        public Task<(IReadOnlyList<DocumentGetDto>, int)> GetAllDocumentsAsync(DocumentParams documentParams);

        public Task<DocumentGetDto> GetDocumentByIdAsync(int id);

        public Task<bool> AddDocumentAsync(DocumentDto documentDto);

        public Task<bool> UpdateDocumentAsync(DocumentDto documentDto);

        public Task<bool> DeleteDocumentAsync(int id);
    }
}
