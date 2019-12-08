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
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly TodoContext _context;
        private readonly IAuthService _authService;

        public ProjectController(ILogger<ProjectController> logger, TodoContext context, IAuthService authService)
        {
            _logger = logger;
            _context = context;
            _authService = authService;
        }
    }
}
