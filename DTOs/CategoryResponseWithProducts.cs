namespace ECommerceApi.DTOs
{
    public class CategoryResponseWithProducts
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<ProductResponse> Products { get; set; } = new();
    }
}