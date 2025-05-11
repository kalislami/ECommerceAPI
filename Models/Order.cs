namespace ECommerceApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public decimal TotalAmount { get; set; }  // Total biaya dari item dalam order
        public string ShippingAddress { get; set; } = string.Empty;

        // Navigasi
        public ICollection<OrderItem> Items { get; set; } = [];
    }
}
