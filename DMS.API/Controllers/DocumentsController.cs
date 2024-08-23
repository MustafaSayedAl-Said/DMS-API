using DMS.API.Helper;
using DMS.Core.Dto;
using DMS.Core.Sharing;
using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    [Route("api/documents")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]

        public async Task<IActionResult> Get([FromQuery] DocumentParams documentParams)
        {
            try
            {
                var (documents, totalItems) = await _documentService.GetAllDocumentsAsync(documentParams);
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

        [HttpPost]

        public async Task<ActionResult> Post([FromForm] DocumentDto documentDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _documentService.AddDocumentAsync(documentDto);
                    if (result)
                        return Ok(documentDto);
                    return BadRequest("Error occurred while adding the document.");
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]

        public async Task<ActionResult> Put([FromForm] DocumentDto documentDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _documentService.UpdateDocumentAsync(documentDto);
                    if (result)
                        return Ok(documentDto);
                    return BadRequest("Error occurred while updating the document.");
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _documentService.DeleteDocumentAsync(id);
                    if (result)
                        return Ok("Document was deleted!");
                    return BadRequest($"Document with ID [{id}] was not found");
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
