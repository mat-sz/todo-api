using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class GenericResponseModel
    {
        [Required]
        public bool Success { get; set; }

        public string Message { get; set; }
    }
}