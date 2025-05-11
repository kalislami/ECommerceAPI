namespace ECommerceApi.Models
{
    public class CartItem
    {
        public int Id { get; set; } // Primary key

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }

        public string UserId { get; set; } = string.Empty;
    }
}
