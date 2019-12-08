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
    public class TodoItemsController : ControllerBase
    {
        private readonly ILogger<TodoItemsController> _logger;
        private readonly TodoContext _context;
        private readonly IAuthService _authService;

        public TodoItemsController(ILogger<TodoItemsController> logger, TodoContext context, IAuthService authService)
        {
            _logger = logger;
            _context = context;
            _authService = authService;
        }

        [HttpPost]
        public IActionResult Create([FromBody]TodoItemModel model)
        {
            var user = _authService.GetUserFromIdentity(this.User.Identity);
            var todoList = _context.TodoLists
                .Include(tl => tl.TodoItems)
                .SingleOrDefault(tl => tl.Id == model.TodoListId);

            if (todoList == null)
                return NotFound(new GenericResponseModel
                    {
                        Success = false,
                        Message = "This item does not exist."
                    });

            var userProject = _context.UserProjects
                .Include(up => up.Project)
                .SingleOrDefault(up => up.ProjectId == todoList.ProjectId && up.UserId == user.Id);

            if (userProject == null)
                return BadRequest(new GenericResponseModel
                    {
                        Success = false,
                        Message = "The project doesn't exist or the current user doesn't have permission to access this project."
                    });

            var item = new TodoItem{
                Name = model.Name,
                Done = model.Done
            };

            todoList.TodoItems.Add(item);
            _context.SaveChanges();

            return Ok(new GenericResponseModel
                {
                    Success = true
                });
        }

        [HttpGet("/{id}")]
        public IActionResult Get(int id)
        {
            var user = _authService.GetUserFromIdentity(this.User.Identity);
            
            var todoItem = _context.TodoItems
                .Include(ti => ti.TodoList)
                .ThenInclude(tl => tl.Project)
                .SingleOrDefault(ti => ti.Id == id);

            if (todoItem == null)
                return NotFound(new GenericResponseModel
                    {
                        Success = false,
                        Message = "This item does not exist."
                    });

            var userProject = _context.UserProjects
                .SingleOrDefault(up => up.ProjectId == todoItem.TodoList.ProjectId && up.UserId == user.Id);

            if (userProject == null)
                return BadRequest(new GenericResponseModel
                    {
                        Success = false,
                        Message = "The project doesn't exist or the current user doesn't have permission to access this project."
                    });

            return Ok(todoItem);
        }

        [HttpPost("/{id}")]
        public IActionResult Update(int id, [FromBody]TodoItemUpdateModel model)
        {
            var user = _authService.GetUserFromIdentity(this.User.Identity);
            
            var todoItem = _context.TodoItems
                .Include(ti => ti.TodoList)
                .ThenInclude(tl => tl.Project)
                .SingleOrDefault(ti => ti.Id == id);

            if (todoItem == null)
                return NotFound(new GenericResponseModel
                    {
                        Success = false,
                        Message = "This item does not exist."
                    });

            var userProject = _context.UserProjects
                .SingleOrDefault(up => up.ProjectId == todoItem.TodoList.ProjectId && up.UserId == user.Id);

            if (userProject == null)
                return BadRequest(new GenericResponseModel
                    {
                        Success = false,
                        Message = "The project doesn't exist or the current user doesn't have permission to access this project."
                    });

            todoItem.Name = model.Name;
            todoItem.Done = model.Done;
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
            
            var todoItem = _context.TodoItems
                .Include(ti => ti.TodoList)
                .ThenInclude(tl => tl.Project)
                .SingleOrDefault(ti => ti.Id == id);

            if (todoItem == null)
                return NotFound(new GenericResponseModel
                    {
                        Success = false,
                        Message = "This item does not exist."
                    });

            var userProject = _context.UserProjects
                .SingleOrDefault(up => up.ProjectId == todoItem.TodoList.ProjectId && up.UserId == user.Id);

            if (userProject == null)
                return BadRequest(new GenericResponseModel
                    {
                        Success = false,
                        Message = "The project doesn't exist or the current user doesn't have permission to access this project."
                    });

            _context.Remove(todoItem);
            _context.SaveChanges();

            return Ok(new GenericResponseModel
                {
                    Success = true
                });
        }
    }
}
