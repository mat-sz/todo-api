using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class ErrorModel
    {
        [Required]
        public string Message { get; set; }
    }
}