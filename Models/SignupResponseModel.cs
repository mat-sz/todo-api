using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class SignupResponseModel
    {
        [Required]
        public bool Success { get; set; }
    }
}