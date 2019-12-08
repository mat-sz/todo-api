using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using TodoAPI.Services;

namespace TodoAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly TodoContext _context;
        private readonly IAuthService _authService;

        public UserController(ILogger<UserController> logger, TodoContext context, IAuthService authService)
        {
            _logger = logger;
            _context = context;
            _authService = authService;
        }
    }
}
