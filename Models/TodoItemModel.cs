using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class TodoItemModel
    {
        [Required]
        public int TodoListId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool Done { get; set; }
    }
}