using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class ProjectModel
    {
        [Required]
        public string Name { get; set; }
    }
}