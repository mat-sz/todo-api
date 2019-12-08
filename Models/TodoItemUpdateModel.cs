using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class TodoItemUpdateModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public bool Done { get; set; }
    }
}