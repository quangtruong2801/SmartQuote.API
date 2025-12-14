using System.ComponentModel.DataAnnotations;

namespace SmartQuote.API.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        // Lưu hash password (BCrypt)
        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Admin"; // User | Admin
    }
}
