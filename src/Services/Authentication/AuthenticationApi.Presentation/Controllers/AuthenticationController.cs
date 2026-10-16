using AuthenticationApi.Application.Dtos;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Repositories;
using BuildingBlocks.Core.Authorization;
using BuildingBlocks.Core.Responses;
using BuildingBlocks.Web.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Response>> Register(AppUserDto appUserDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _userRepository.Register(appUserDTO);
            return result.flag ? Ok(result) : BadRequest(Request);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response>> Login(LoginDto loginDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _userRepository.Login(loginDTO);
            return result.flag ? Ok(result) : BadRequest(Request);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Response>> GetUser(int id)
        {
            if(id <= 0 ) return BadRequest("Invalid user Id");
            var user = await _userRepository.GetUser(id);
            return user.Id > 0 ? Ok(user) : NotFound(user);
        }

        [HttpPost("permissions")]
        [CustomAuthorize(FeatureConstants.UserManagement, PermissionLevel.FullAccess)]
        public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionDto dto)
        {
            var response = await _userRepository.AssignPermission(dto);
            return response.flag? Ok(response) : BadRequest(response);
        }
    }
}
