using System.ComponentModel.DataAnnotations;

namespace SafeVault.Api.Models
{
    public class UserAccount
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; } // "User", "Admin"
    }
}
