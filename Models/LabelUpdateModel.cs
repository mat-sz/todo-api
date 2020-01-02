using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class LabelUpdateModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Color { get; set; }
    }
}