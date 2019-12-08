using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class SignupRequestModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}