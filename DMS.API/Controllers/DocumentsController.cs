using DMS.API.Helpers;
using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Sharing;
using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Security.Claims;

namespace DMS.API.Controllers
{
    [Route("api/documents")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IActionLogService _actionLogService;

        public DocumentsController(IDocumentService documentService, IRabbitMQService rabbitMQService, IActionLogService actionLogService)
        {
            _documentService = documentService;
            _actionLogService = actionLogService;
            _rabbitMQService = rabbitMQService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] DocumentParams documentParams)
        {
            try
            {
                var userId = HttpContext.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                // Check if the user has the "Admin" role from the token
                var isAdmin = HttpContext.User.IsInRole("Admin");

                if (!isAdmin)
                {
                    // Check if the directory belongs to the user
                    var isOwner = await _documentService.VerifyDirectoryOwnershipAsync(documentParams.DirectoryId, int.Parse(userId));
                    if (!isOwner)
                    {
                        return Forbid("User is not authorized to access this directory");
                    }
                }


                var (documents, totalItems) = await _documentService.GetAllDocumentsAsync(documentParams);
                return Ok(new Pagination<DocumentGetDto>(totalItems, documentParams.PageSize, documentParams.PageNumber, documents));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("public")]

        public async Task<IActionResult> GetPublic([FromQuery] DocumentParams documentParams)
        {
            try
            {
                var (documents, totalItems) = await _documentService.GetAllPublicDocumentsAsync(documentParams);
                return Ok(new Pagination<DocumentGetDto>(totalItems, documentParams.PageSize, documentParams.PageNumber, documents));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var document = await _documentService.GetDocumentByIdAsync(id);
                if (document != null)
                {
                    return Ok(document);
                }
                return NotFound($"Document with ID [{id}] was not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // upload document
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Post([FromForm] DocumentDto documentDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                    var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

                    // Check if the user has the "Admin" role from the token
                    var isAdmin = HttpContext.User.IsInRole("Admin");

                    if (!isAdmin)
                    {
                        var isOwner = await _documentService.VerifyDirectoryOwnershipAsync(documentDto.DirectoryId, int.Parse(userId));
                        if (!isOwner)
                        {
                            return Forbid("User is not authorized to access this directory");
                        }
                    }
                    var result = await _documentService.AddDocumentAsync(documentDto, email);
                    if (result)
                    {
                        // create a log
                        var logEntry = new ActionLog
                        {
                            UserId = int.Parse(userId),
                            UserName = email,
                            ActionType = ActionTypeEnum.Upload,
                            CreationDate = DateTime.Now,
                            DocumentId = null,
                            DocumentName = documentDto.DocumentContent.FileName,
                        };
                        _rabbitMQService.SendMessage(logEntry);
                        await _actionLogService.AddActionLogAsync(logEntry);
                        return Ok(documentDto);
                    }
                    return BadRequest("Error occurred while adding the document.");
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{id}")]
        [Authorize]

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userId = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

                // Check if the user has the "Admin" role from the token
                var isAdmin = HttpContext.User.IsInRole("Admin");

                if (!isAdmin)
                {
                    var isOwner = await _documentService.VerifyDocumentOwnershipAsync(id, email);

                    if (!isOwner)
                    {
                        return Forbid("User is not authorized to access this directory");
                    }
                }

                var result = await _documentService.SoftDeleteDocumentAsync(id);

                if (result)
                    return Ok("Document was soft deleted successfully");
                return BadRequest("Error occurred while deleting the document");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        [Authorize]

        public async Task<ActionResult> Patch(int id)
        {
            try
            {
                var userId = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

                // Check if the user has the "Admin" role from the token
                var isAdmin = HttpContext.User.IsInRole("Admin");

                if (!isAdmin)
                {
                    var isOwner = await _documentService.VerifyDocumentOwnershipAsync(id, email);

                    if (!isOwner)
                    {
                        return Forbid("User is not authorized to modify this document");
                    }
                }

                var result = await _documentService.UpdateDocumentVisibilityAsync(id);

                if (!result)
                {
                    return BadRequest("Error occurred while updating the document visibility");
                }

                return Ok("Document visibility updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // download document
        [HttpGet("download/{id}")]

        public async Task<IActionResult> DownloadFile(int id)
        {
            try
            {
                var userId = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                var documentDto = await _documentService.GetDocumentByIdAsync(id);

                if (documentDto == null)
                {
                    return NotFound("Document not found");
                }

                // Check if document is public or not
                if (!documentDto.IsPublic)
                {
                    if (string.IsNullOrEmpty(userId))
                    {
                        return Unauthorized("User is not authenticated or user ID is invalid");
                    }

                    // Check if the user has the "Admin" role from the token
                    var isAdmin = HttpContext.User.IsInRole("Admin");

                    if (!isAdmin)
                    {
                        var isOwner = await _documentService.VerifyDocumentOwnershipAsync(id, email);

                        if (!isOwner)
                        {
                            return Forbid("User is not authorized to download this document");
                        }
                    }
                }
                var path = "wwwroot" + documentDto.DocumentContent;


                var net = new System.Net.WebClient();
                var data = net.DownloadData(path);
                var content = new System.IO.MemoryStream(data);
                var contentType = "APPLICATION/octet-stream";
                var fileName = documentDto.Name;


                // create a log

                var logEntry = new ActionLog
                {
                    UserId = string.IsNullOrEmpty(userId) ? null : int.Parse(userId),
                    UserName = string.IsNullOrEmpty(email) ? "Public User" : email,
                    ActionType = ActionTypeEnum.Download,
                    CreationDate = DateTime.Now,
                    DocumentId = id,
                    DocumentName = documentDto.Name,
                };
                _rabbitMQService.SendMessage(logEntry);
                await _actionLogService.AddActionLogAsync(logEntry);

                return File(content, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error downloading file: {ex.Message}");
            }
        }

        //preview document

        [HttpGet("preview/{id}")]
        public async Task<IActionResult> PreviewFile(int id)
        {
            try
            {
                var userId = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                var documentDto = await _documentService.GetDocumentByIdAsync(id);
                var isAdmin = HttpContext.User.IsInRole("Admin");

                if (!documentDto.IsPublic)
                {

                    if (string.IsNullOrEmpty(userId))
                    {
                        return Unauthorized("User is not authenticated or user ID is invalid");
                    }

                    if (!isAdmin)
                    {
                        if (documentDto == null || (documentDto.OwnerName != email))
                        {
                            return Forbid("You don't have access to this file.");
                        }
                    }
                }

                var path = "wwwroot" + documentDto.DocumentContent;
                if (!System.IO.File.Exists(path))
                {
                    return NotFound("File not found");
                }

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(path, out var contentType))
                {
                    contentType = "application/octet-stream"; // Default to binary stream if unknown
                }

                var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

                // create a log
                var logEntry = new ActionLog
                {
                    UserId = string.IsNullOrEmpty(userId)? null:int.Parse(userId),
                    UserName = string.IsNullOrEmpty(email)? "Public User":email,
                    ActionType = ActionTypeEnum.Preview,
                    CreationDate = DateTime.Now,
                    DocumentId = id,
                    DocumentName = documentDto.Name,
                };
                _rabbitMQService.SendMessage(logEntry);
                await _actionLogService.AddActionLogAsync(logEntry);
                return File(fileStream, contentType);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error previewing file: {ex.Message}");
            }
        }

    }
}
