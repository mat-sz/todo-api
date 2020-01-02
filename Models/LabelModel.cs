using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class LabelModel
    {
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Color { get; set; }
    }
}