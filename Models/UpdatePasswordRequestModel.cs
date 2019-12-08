using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models
{
    public class UpdatePasswordRequestModel
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string Password { get; set; }
    }
}