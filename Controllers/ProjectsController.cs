using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using TodoAPI.Entities;
using TodoAPI.Models;
using TodoAPI.Services;

namespace TodoAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ILogger<ProjectsController> _logger;
        private readonly TodoContext _context;
        private readonly IAuthService _authService;

        public ProjectsController(ILogger<ProjectsController> logger, TodoContext context, IAuthService authService)
        {
            _logger = logger;
            _context = context;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var user = _authService.GetUserFromIdentity(this.User.Identity);
            var userProjects = _context.UserProjects
                .Include(up => up.Project)
                .ThenInclude(p => p.TodoLists)
                .Where(up => up.UserId == user.Id);
            
            return Ok(userProjects.Select(up => up.Project));
        }

        [HttpPost]
        public IActionResult Create([FromBody]ProjectModel model)
        {
            var user = _authService.GetUserFromIdentity(this.User.Identity);
            var project = new Project{
                Name = model.Name
            };
            var userProject = new UserProject{
                User = user
            };

            project.UserProjects.Add(userProject);
            _context.Add(project);
            _context.SaveChanges();

            return Ok(new GenericResponseModel
                {
                    Success = true
                });
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var user = _authService.GetUserFromIdentity(this.User.Identity);
            var userProject = _context.UserProjects
                .Include(up => up.Project)
                .ThenInclude(p => p.TodoLists)
                .ThenInclude(tl => tl.TodoItems)
                .SingleOrDefault(up => up.ProjectId == id && up.UserId == user.Id);

            if (userProject == null)
                return BadRequest(new GenericResponseModel
                    {
                        Success = false,
                        Message = "The project doesn't exist or the current user doesn't have permission to access this project."
                    });

            return Ok(userProject.Project);
        }

        [HttpPost("/{id}")]
        public IActionResult Update(int id, [FromBody]ProjectModel model)
        {
            var user = _authService.GetUserFromIdentity(this.User.Identity);
            var userProject = _context.UserProjects
                .Include(up => up.Project)
                .SingleOrDefault(up => up.ProjectId == id && up.UserId == user.Id);

            if (userProject == null)
                return BadRequest(new GenericResponseModel
                    {
                        Success = false,
                        Message = "The project doesn't exist or the current user doesn't have permission to access this project."
                    });

            userProject.Project.Name = model.Name;
            _context.SaveChanges();

            return Ok(new GenericResponseModel
                {
                    Success = true
                });
        }

        [HttpDelete("/{id}")]
        public IActionResult Delete(int id)
        {
            var user = _authService.GetUserFromIdentity(this.User.Identity);
            var userProject = _context.UserProjects
                .Include(up => up.Project)
                .SingleOrDefault(up => up.ProjectId == id && up.UserId == user.Id);

            if (userProject == null)
                return BadRequest(new GenericResponseModel
                    {
                        Success = false,
                        Message = "The project doesn't exist or the current user doesn't have permission to access this project."
                    });

            _context.Remove(userProject.Project);
            _context.SaveChanges();

            return Ok(new GenericResponseModel
                {
                    Success = true
                });
        }
    }
}
