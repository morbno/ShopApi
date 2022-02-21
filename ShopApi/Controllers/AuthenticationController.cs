using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Models;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthenticationController(ShopContext context)
        {
            _userService = new UserService(context);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateModel model)
        {
            var token = await _userService.Authenticate(model.Username, model.Password);

            if (token is null)
                return BadRequest("Некорректный логин или пароль");

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(AuthenticateModel model)
        {
            var registerResult = await _userService.Register(model.Username, model.Password);

            return registerResult == "" 
                ? Ok() 
                : BadRequest(registerResult);
        }
    }
}
