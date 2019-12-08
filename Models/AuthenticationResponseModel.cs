using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class AuthenticationResponseModel
    {
        [Required]
        public bool Success { get; set; }

        public string Token { get; set; }
    }
}