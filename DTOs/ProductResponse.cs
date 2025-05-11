namespace ECommerceApi.DTOs
{
    public class ProductResponse
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public required int Stock { get; set; }
        public required int CategoryId { get; set; }
    }
}