using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [MaxLength(255)]
        public string Username { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Role { get; set; } = "user";
    }
}