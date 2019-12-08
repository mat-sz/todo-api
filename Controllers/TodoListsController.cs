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
    public class TodoListsController : ControllerBase
    {
        private readonly ILogger<TodoListsController> _logger;
        private readonly TodoContext _context;
        private readonly IAuthService _authService;

        public TodoListsController(ILogger<TodoListsController> logger, TodoContext context, IAuthService authService)
        {
            _logger = logger;
            _context = context;
            _authService = authService;
        }

        [HttpPost]
        public IActionResult Create([FromBody]TodoListModel model)
        {
            var user = _authService.GetUserFromIdentity(this.User.Identity);
            var userProject = _context.UserProjects
                .Include(up => up.Project)
                .SingleOrDefault(up => up.ProjectId == model.ProjectId && up.UserId == user.Id);

            if (userProject == null)
                return BadRequest(new { message = "The project doesn't exist or the current user doesn't have permission to access this project." });

            var list = new TodoList{
                Name = model.Name
            };

            userProject.Project.TodoLists.Add(list);
            _context.SaveChanges();

            return Ok(new { success = true });
        }

        [HttpGet("/{id}")]
        public IActionResult Get(int id)
        {
            var user = _authService.GetUserFromIdentity(this.User.Identity);
            var todoList = _context.TodoLists
                .Include(tl => tl.TodoItems)
                .SingleOrDefault(tl => tl.Id == id);

            if (todoList == null)
                return NotFound(new { message = "This list does not exist." });

            var userProject = _context.UserProjects
                .SingleOrDefault(up => up.ProjectId == todoList.ProjectId && up.UserId == user.Id);

            if (userProject == null)
                return BadRequest(new { message = "The project doesn't exist or the current user doesn't have permission to access this project." });

            return Ok(todoList);
        }

        [HttpPost("/{id}")]
        public IActionResult Update(int id, [FromBody]TodoListUpdateModel model)
        {
            var user = _authService.GetUserFromIdentity(this.User.Identity);
            var todoList = _context.TodoLists
                .SingleOrDefault(tl => tl.Id == id);

            if (todoList == null)
                return NotFound(new { message = "This list does not exist." });

            var userProject = _context.UserProjects
                .SingleOrDefault(up => up.ProjectId == todoList.ProjectId && up.UserId == user.Id);

            if (userProject == null)
                return BadRequest(new { message = "The project doesn't exist or the current user doesn't have permission to access this project." });

            todoList.Name = model.Name;
            _context.SaveChanges();

            return Ok(new { success = true });
        }

        [HttpDelete("/{id}")]
        public IActionResult Delete(int id)
        {
            var user = _authService.GetUserFromIdentity(this.User.Identity);
            var todoList = _context.TodoLists
                .SingleOrDefault(tl => tl.Id == id);

            if (todoList == null)
                return NotFound(new { message = "This list does not exist." });

            var userProject = _context.UserProjects
                .SingleOrDefault(up => up.ProjectId == todoList.ProjectId && up.UserId == user.Id);

            if (userProject == null)
                return BadRequest(new { message = "The project doesn't exist or the current user doesn't have permission to access this project." });

            _context.Remove(todoList);
            _context.SaveChanges();

            return Ok(new { success = true });
        }
    }
}
