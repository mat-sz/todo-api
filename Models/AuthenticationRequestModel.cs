using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class AuthenticationRequestModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}