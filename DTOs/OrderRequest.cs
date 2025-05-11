namespace ECommerceApi.DTOs
{
    public class OrderRequest
    {
        public List<OrderItemRequest> Items { get; set; } = [];
    }

    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}