using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class ResponseModel
    {
        [Required]
        public bool Success { get; set; }

        public ErrorModel Error { get; set; }

        public object Data { get; set; }
    }
}