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
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Authenticate([FromBody]AuthenticationRequestModel model)
        {
            var responseModel = _authService.Authenticate(model.Username, model.Password);

            if (!responseModel.Success)
                return BadRequest(responseModel);

            return Ok(responseModel);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Status()
        {
            if (!this.User.Identity.IsAuthenticated) {
                return Ok(new ResponseModel {
                    Success = true,
                    Data = null
                });
            }
            
            var user = _authService.GetUserFromIdentity(this.User.Identity);
            return Ok(new ResponseModel {
                Success = true,
                Data = user
            });
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public IActionResult Signup([FromBody]SignupModel model)
        {
            var success = _authService.CreateUser(model.Username, model.Password);

            if (!success) {
                return BadRequest(new ResponseModel {
                    Success = false,
                    Error = new ErrorModel {
                        Message = "An user with this username already exists."
                    }
                });
            }

            return Ok(new ResponseModel {
                Success = true
            });
        }

        [HttpPost("password")]
        public IActionResult Password([FromBody]PasswordUpdateRequestModel model)
        {
            var user = _authService.GetUserFromIdentity(this.User.Identity);

            if (user == null) {
                return BadRequest(new ResponseModel {
                    Success = false,
                    Error = new ErrorModel {
                        Message = "Malformed token."
                    }
                });
            }

            var success = _authService.UpdatePassword(user, model.OldPassword, model.Password);

            if (!success) {
                return BadRequest(new ResponseModel {
                    Success = false,
                    Error = new ErrorModel {
                        Message = "The provided password is not correct."
                    }
                });
            }

            return Ok(new ResponseModel {
                Success = true
            });
        }
    }
}
