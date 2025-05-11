namespace ECommerceApi.DTOs
{
    public class PaymentRequest
    {
        public required int OrderId { get; set; }
        public required decimal GrossAmount { get; set; }
        public string CustomerEmail { get; set; } = "";
    }
}
