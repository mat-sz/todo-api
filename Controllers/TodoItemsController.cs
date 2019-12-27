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

            if (todoList == null) {
                return NotFound(new ResponseModel {
                    Success = false,
                    Error = new ErrorModel {
                        Message = "This item does not exist."
                    }
                });
            }

            var userProject = _context.UserProjects
                .Include(up => up.Project)
                .SingleOrDefault(up => up.ProjectId == todoList.ProjectId && up.UserId == user.Id);

            if (userProject == null) {
                return BadRequest(new ResponseModel {
                    Success = false,
                    Error = new ErrorModel {
                        Message = "The project doesn't exist or the current user doesn't have permission to access this project."
                    }
                });
            }

            var item = new TodoItem {
                Name = model.Name,
                Done = model.Done
            };

            todoList.TodoItems.Add(item);
            _context.SaveChanges();

            return Ok(new ResponseModel {
                Success = true
            });
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(TodoItemsActionFilter))]
        public IActionResult Get(int id)
        {
            TodoItem todoItem = (TodoItem)HttpContext.Items["todoItem"];
            return Ok(new ResponseModel {
                Success = true,
                Data = todoItem
            });
        }

        [HttpPost("{id}")]
        [TypeFilter(typeof(TodoItemsActionFilter))]
        public IActionResult Update(int id, [FromBody]TodoItemUpdateModel model)
        {
            TodoItem todoItem = (TodoItem)HttpContext.Items["todoItem"];
            todoItem.Name = model.Name;
            todoItem.Done = model.Done;
            _context.SaveChanges();

            return Ok(new ResponseModel {
                Success = true
            });
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(TodoItemsActionFilter))]
        public IActionResult Delete(int id)
        {
            TodoItem todoItem = (TodoItem)HttpContext.Items["todoItem"];
            _context.Remove(todoItem);
            _context.SaveChanges();

            return Ok(new ResponseModel {
                Success = true
            });
        }
    }
}
