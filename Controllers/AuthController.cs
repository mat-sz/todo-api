using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using TodoAPI.Models;
using TodoAPI.Services;

namespace TodoAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authenticationService;

        public AuthController(ILogger<AuthController> logger, IAuthService authenticationService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Authenticate([FromBody]AuthenticationRequestModel model)
        {
            var responseModel = _authenticationService.Authenticate(model.Username, model.Password);

            if (!responseModel.Success)
                return BadRequest(responseModel);

            return Ok(responseModel);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Status()
        {
            return Ok(this.User);
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public IActionResult Signup([FromBody]SignupRequestModel model)
        {
            var responseModel = _authenticationService.CreateUser(model.Username, model.Password);

            if (!responseModel.Success)
                return BadRequest(responseModel);

            return Ok(responseModel);
        }

        [HttpPost("password")]
        public IActionResult Password([FromBody]UpdatePasswordRequestModel model)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            int userId;

            if (!Int32.TryParse(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value, out userId))
                return BadRequest(new { message = "Malformed token." });

            var success = _authenticationService.UpdatePassword(userId, model.OldPassword, model.Password);

            if (!success)
                return BadRequest(new { message = "The provided password is not correct." });

            return Ok(new { success = true });
        }
    }
}
