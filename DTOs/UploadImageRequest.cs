namespace ECommerceApi.DTOs
{
    public class UploadImageRequest
    {
        public required IFormFile Image { get; set; }
        public required int ProductId { get; set; }
    }
}
