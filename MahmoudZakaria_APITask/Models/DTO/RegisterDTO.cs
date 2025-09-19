using System.ComponentModel.DataAnnotations;

namespace MahmoudZakaria_APITask.Models.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public string Role { get; set; }= "User";
    }
}
