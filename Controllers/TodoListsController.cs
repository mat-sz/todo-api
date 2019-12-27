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

            if (userProject == null) {
                return BadRequest(new ResponseModel {
                    Success = false,
                    Error = new ErrorModel {
                        Message = "The project doesn't exist or the current user doesn't have permission to access this project."
                    }
                });
            }

            var list = new TodoList{
                Name = model.Name
            };

            userProject.Project.TodoLists.Add(list);
            _context.SaveChanges();

            return Ok(new ResponseModel {
                Success = true
            });
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(TodoListsActionFilter))]
        public IActionResult Get(int id)
        {
            TodoList todoList = (TodoList)HttpContext.Items["todoList"];
            return Ok(new ResponseModel {
                Success = true,
                Data = todoList
            });
        }

        [HttpPost("{id}")]
        [TypeFilter(typeof(TodoListsActionFilter))]
        public IActionResult Update(int id, [FromBody]TodoListUpdateModel model)
        {
            TodoList todoList = (TodoList)HttpContext.Items["todoList"];
            todoList.Name = model.Name;
            _context.SaveChanges();

            return Ok(new ResponseModel {
                Success = true
            });
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(TodoListsActionFilter))]
        public IActionResult Delete(int id)
        {
            TodoList todoList = (TodoList)HttpContext.Items["todoList"];
            _context.Remove(todoList);
            _context.SaveChanges();

            return Ok(new ResponseModel {
                Success = true
            });
        }
    }
}
