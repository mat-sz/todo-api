using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class TodoListUpdateModel
    {
        [Required]
        public string Name { get; set; }
    }
}