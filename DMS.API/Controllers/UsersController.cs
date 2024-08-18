using AutoMapper;
using DMS.Core.Dto;
using DMS.Core.Entities;
using DMS.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DMS.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _uOW;
        private readonly IMapper _mapper;

        public UsersController(IUnitOfWork UOW, IMapper mapper)
        {
            _uOW = UOW;
            _mapper = mapper;
        }

        [HttpGet]

        public async Task<IActionResult> Get()
        {
            var allUsers = await _uOW.userRepository.GetAllAsync();
            var users = _mapper.Map<List<UserDto>>(allUsers);

            if (allUsers is not null)
            {
                return Ok(users);
            }
            return BadRequest("Not Found");
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> Get(int id)
        {
            var user = await _uOW.userRepository.GetAsync(id);
            var userDto = _mapper.Map<UserDto>(user);
            if (userDto is not null)
            {
                return Ok(userDto);
            }
            return BadRequest($"This id [{id}] was Not Found");
        }

        [HttpPost]

        public async Task<ActionResult> Post(UserDto userDto, string WorkspaceName)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var UserMap = _mapper.Map<User>(userDto);
                    await _uOW.userRepository.AddAsync(UserMap);

                    var workspaceEntity = new Workspace
                    {
                        Name = WorkspaceName,
                        UserId = UserMap.Id
                    };
                    await _uOW.workspaceRepository.AddAsync(workspaceEntity);
                    return Ok("User Created");
                }

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]

        public async Task<ActionResult> Put(UserDto userDto)
        {
            try
            {
                if (userDto == null)
                    return BadRequest(ModelState);
                if (ModelState.IsValid)
                {
                    if (_uOW.userRepository.userExists(userDto.id))
                    {
                        var userMap = _mapper.Map<User>(userDto);
                        await _uOW.userRepository.UpdateAsync(userMap);
                        return Ok(userDto);
                    }
                    return BadRequest($"User Not Foun, Id [{userDto.id}] is Incorrect");
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
                    if (_uOW.userRepository.userExists(id))
                    {
                        await _uOW.userRepository.DeleteAsync(id);
                        return Ok("User was deleted");
                    }
                    return BadRequest($"User was Not Found, Id [{id}] Incorrect");
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
