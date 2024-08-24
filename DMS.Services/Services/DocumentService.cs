using AutoMapper;
using DMS.Core.Dto;
using DMS.Core.Interfaces;
using DMS.Core.Sharing;
using DMS.Services.Interfaces;

namespace DMS.Services.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _uOW;
        private readonly IMapper _mapper;

        public DocumentService(IUnitOfWork UOW, IMapper mapper)
        {
            _uOW = UOW;
            _mapper = mapper;
        }

        public async Task<(IReadOnlyList<DocumentGetDto>, int)> GetAllDocumentsAsync(DocumentParams documentParams)
        {
            if (_uOW.directoryRepository.directoryExists(documentParams.DirectoryId))
            {
                var (allDocuments, totalItems) = await _uOW.documentRepository.GetAllAsync(documentParams);
                var documents = _mapper.Map<IReadOnlyList<DocumentGetDto>>(allDocuments);

                if (documents is not null)
                {
                    var directory = await _uOW.directoryRepository.GetAsync(documentParams.DirectoryId);
                    var userId = _uOW.workspaceRepository.getUserId(directory.WorkspaceId);
                    var User = await _uOW.userRepository.GetAsync(userId);
                    foreach (var document in documents)
                    {
                        document.UserName = User.Email;
                    }
                    return (documents, totalItems);
                }
                throw new Exception("Error while retrieving documents");
            }
            throw new Exception("Directory doesn't exist");
        }

        public async Task<DocumentGetDto> GetDocumentByIdAsync(int id)
        {
            var document = await _uOW.documentRepository.GetAsync(id);
            return _mapper.Map<DocumentGetDto>(document);
        }

        public async Task<bool> AddDocumentAsync(DocumentDto documentDto)
        {
            if (_uOW.directoryRepository.directoryExists(documentDto.DirectoryId))
            {
                return await _uOW.documentRepository.AddSync(documentDto);
            }
            throw new Exception("Directory doesn't Exist");
        }

        public async Task<bool> UpdateDocumentAsync(DocumentDto documentDto)
        {
            if (documentDto == null)
                throw new ArgumentNullException(nameof(documentDto));
            if (_uOW.documentRepository.documentExists(documentDto.Id))
            {
                return await _uOW.documentRepository.UpdateAsync(documentDto);
            }
            throw new Exception($"Document Not Found, Id [{documentDto.Id}] is Incorrect");
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            if (_uOW.documentRepository.documentExists(id))
            {
                await _uOW.documentRepository.DeleteAsync(id);
                return true;
            }
            throw new Exception($"Document Not Found, Id [{id}] is Incorrect");
        }
    }
}
