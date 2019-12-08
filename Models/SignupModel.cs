using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class SignupModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}