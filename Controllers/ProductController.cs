using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerceApi.Data;
using ECommerceApi.Models;
using Microsoft.EntityFrameworkCore;
using ECommerceApi.Services;
using ECommerceApi.DTOs;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly FirebaseStorageService _firebaseStorageService;

        public ProductController(AppDbContext context, FirebaseStorageService firebaseStorageService)
        {
            _context = context;
            _firebaseStorageService = firebaseStorageService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return product;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return StatusCode(201, new { message = "Product created" });
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product updatedProduct)
        {
            if (id != updatedProduct.Id) return BadRequest();

            _context.Entry(updatedProduct).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
        {
            var file = request.Image;

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var fileName = Path.GetFileName(file.FileName);

            using var stream = file.OpenReadStream();
            var imageUrl = await _firebaseStorageService.UploadImageAsync(stream, fileName);

            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            product.ImageUrl = imageUrl;
            await _context.SaveChangesAsync();

            return Ok(new { ImageUrl = imageUrl });
        }
    }
}
