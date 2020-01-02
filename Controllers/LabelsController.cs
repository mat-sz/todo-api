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
    public class LabelsController : ControllerBase
    {
        private readonly ILogger<LabelsController> _logger;
        private readonly TodoContext _context;
        private readonly IAuthService _authService;

        public LabelsController(ILogger<LabelsController> logger, TodoContext context, IAuthService authService)
        {
            _logger = logger;
            _context = context;
            _authService = authService;
        }

        [HttpPost]
        public IActionResult Create([FromBody]LabelModel model)
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

            var label = new Label{
                Name = model.Name,
                Color = model.Color,
            };

            userProject.Project.Labels.Add(label);
            _context.SaveChanges();

            return Ok(new ResponseModel {
                Success = true
            });
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(LabelsActionFilter))]
        public IActionResult Get(int id)
        {
            Label label = (Label)HttpContext.Items["label"];
            return Ok(new ResponseModel {
                Success = true,
                Data = label
            });
        }

        [HttpPost("{id}")]
        [TypeFilter(typeof(LabelsActionFilter))]
        public IActionResult Update(int id, [FromBody]LabelUpdateModel model)
        {
            Label label = (Label)HttpContext.Items["label"];
            label.Name = model.Name;
            label.Color = model.Color;
            _context.SaveChanges();

            return Ok(new ResponseModel {
                Success = true
            });
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(LabelsActionFilter))]
        public IActionResult Delete(int id)
        {
            Label label = (Label)HttpContext.Items["label"];
            _context.Remove(label);
            _context.SaveChanges();

            return Ok(new ResponseModel {
                Success = true
            });
        }
    }
}
