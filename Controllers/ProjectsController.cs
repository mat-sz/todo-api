using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using TodoAPI.Entities;
using TodoAPI.Filters;
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
            
            return Ok(new ResponseModel {
                Success = true,
                Data = userProjects.Select(up => up.Project)
            });
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

            return Ok(new ResponseModel {
                Success = true
            });
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(ProjectsActionFilter))]
        public IActionResult Get(int id)
        {
            Project project = (Project)HttpContext.Items["project"];
            return Ok(new ResponseModel {
                Success = true,
                Data = project
            });
        }

        [HttpPost("{id}")]
        [TypeFilter(typeof(ProjectsActionFilter))]
        public IActionResult Update(int id, [FromBody]ProjectModel model)
        {
            Project project = (Project)HttpContext.Items["project"];
            project.Name = model.Name;
            _context.SaveChanges();

            return Ok(new ResponseModel {
                Success = true
            });
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(ProjectsActionFilter))]
        public IActionResult Delete(int id)
        {
            Project project = (Project)HttpContext.Items["project"];
            _context.Remove(project);
            _context.SaveChanges();

            return Ok(new ResponseModel {
                Success = true
            });
        }
    }
}
