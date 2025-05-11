namespace ECommerceApi.DTOs
{
    public class OrderStatusRequest
    {
        public string Status { get; set; } = "Cancel";
        public required int OrderId { get; set; }
    }
}
