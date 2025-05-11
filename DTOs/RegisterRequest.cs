namespace ECommerceApi.DTOs
{
    public class RegisterRequest
    {
        public string? Username { get; set; }  // Username optional
        public required string Email { get; set; }      // Email wajib
        public required string Password { get; set; }   // Password
        public string Role { get; set; } = "User"; // Default role adalah "User"
    }
}