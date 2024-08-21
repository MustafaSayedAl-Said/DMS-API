using AutoMapper;
using DMS.Core.Sharing;
using DMS.Core.Dto;
using DMS.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using DMS.API.Helper;

namespace DMS.API.Controllers
{
    [Route("api/documents")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IUnitOfWork _uOW;
        private readonly IMapper _mapper;

        public DocumentsController(IUnitOfWork UOW, IMapper mapper)
        {
            _uOW = UOW;
            _mapper = mapper;
        }

        [HttpGet]

        public async Task<IActionResult> Get([FromQuery]DocumentParams documentParams)
        {
            var allDocuments = await _uOW.documentRepository.GetAllAsync(documentParams);
            var documents = _mapper.Map<IReadOnlyList<DocumentGetDto>>(allDocuments);
            var totalItems = documents.Count;

            if (documents is not null)
            {
                return Ok(new Pagination<DocumentGetDto>(totalItems, documentParams.PageSize, documentParams.PageNumber, documents));
            }

            return BadRequest();
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> Get(int id)
        {
            var document = await _uOW.documentRepository.GetAsync(id);
            var documentDto = _mapper.Map<DocumentGetDto>(document);
            if (documentDto is not null)
            {
                return Ok(documentDto);
            }
            return BadRequest($"This id [{id}] was Not Found");
        }

        //[HttpGet("directory/{directoryId}")]

        //public async Task<IActionResult> DocumentsInDirectory(int directoryId)
        //{
        //    if (_uOW.directoryRepository.directoryExists(directoryId))
        //    {
        //        var documents = await _uOW.documentRepository.GetDocumentsInDirectory(directoryId);
        //        var documentsMap = _mapper.Map<List<DocumentGetDto>>(documents);
        //        if (documentsMap is not null)
        //        {
        //            return Ok(documentsMap);
        //        }
        //        return BadRequest("Something went wrong");
        //    }
        //    return BadRequest("Directory doesn't exist");
        //}

        [HttpPost]

        public async Task<ActionResult> Post([FromForm] DocumentDto documentDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_uOW.directoryRepository.directoryExists(documentDto.DirectoryId))
                    {
                        var res = await _uOW.documentRepository.AddSync(documentDto);
                        return res ? Ok(documentDto) : BadRequest("Something went wrong");
                    }
                    return NotFound("Directory Was Not Found");
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
                if (documentDto is null)
                    return BadRequest(ModelState);
                if (ModelState.IsValid)
                {
                    if (_uOW.documentRepository.documentExists(documentDto.Id))
                    {
                        var res = await _uOW.documentRepository.UpdateAsync(documentDto);
                        return res ? Ok(documentDto) : BadRequest("Something went wrong");
                    }
                    return BadRequest($"Document Not Found, Id [{documentDto.Id}] is Incorrect");
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
                    if (_uOW.documentRepository.documentExists(id))
                    {
                        await _uOW.documentRepository.DeleteAsync(id);
                        return Ok("Document was deleted!");
                    }
                    return BadRequest($"Document Not Found, Id [{id}] is Incorrect");
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
