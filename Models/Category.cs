namespace ECommerceApi.Models
{
    public class Category
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; } = string.Empty;

        // Navigasi ke produk
        public ICollection<Product>? Products { get; set; }
    }
}