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
    public class TodoController : ControllerBase
    {
        private readonly ILogger<TodoController> _logger;
        private readonly TodoContext _context;
        private readonly IAuthService _authService;

        public TodoController(ILogger<TodoController> logger, TodoContext context, IAuthService authService)
        {
            _logger = logger;
            _context = context;
            _authService = authService;
        }
    }
}
