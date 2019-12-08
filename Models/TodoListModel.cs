using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class TodoListModel
    {
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}