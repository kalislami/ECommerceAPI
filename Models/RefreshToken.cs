namespace ECommerceApi.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public int UserId { get; set; }
        public User User { get; set; } = default!;
        public DateTime CreatedAt { get; internal set; }
    }
}