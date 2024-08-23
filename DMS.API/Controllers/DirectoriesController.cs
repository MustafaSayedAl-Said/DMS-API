using DMS.API.Helper;
using DMS.Core.Dto;
using DMS.Core.Sharing;
using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    [Route("api/directories")]
    [ApiController]
    public class DirectoriesController : ControllerBase
    {
        private readonly IDirectoryService _directoryService;
        public DirectoriesController(IDirectoryService directoryService)
        {
            _directoryService = directoryService;
        }

        [HttpGet]

        public async Task<IActionResult> Get([FromQuery] DirectoryParams directoryParams)
        {
            try
            {
                var (directories, totalItems) = await _directoryService.GetAllDirectoriesAsync(directoryParams);
                return Ok(new Pagination<MyDirectoryDto>(totalItems, directoryParams.PageSize, directoryParams.PageNumber, directories));
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
                var directoryDto = await _directoryService.GetDirectoryByIdAsync(id);
                return Ok(directoryDto);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]

        public async Task<ActionResult> Post(MyDirectoryDto directoryDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _directoryService.AddDirectoryAsync(directoryDto);
                    if (result)
                        return Ok(directoryDto);

                    return BadRequest("Error occurred while adding the directory");
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]

        public async Task<ActionResult> Put(MyDirectoryDto directoryDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _directoryService.UpdateDirectoryAsync(directoryDto);
                    if (result)
                        return Ok(directoryDto);
                    return BadRequest("Error occurred while updating the directory");
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
                    var result = await _directoryService.DeleteDirectoryAsync(id);
                    if (result)
                        return Ok("Directory was deleted!");
                    return BadRequest("Error occurred while deleting the directory");
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
