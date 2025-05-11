using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceApi.Data;
using ECommerceApi.Models;
using ECommerceApi.DTOs;
using System.Security.Claims;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        private string? GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemResponse>>> GetCartItems()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("User ID tidak ditemukan dalam token.");

            var items = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .Select(c => new CartItemResponse
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product!.Name,
                    Quantity = c.Quantity
                })
                .ToListAsync();

            return Ok(items);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddToCart([FromBody] CartItemRequest request)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("User ID tidak ditemukan dalam token.");

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                var newItem = new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UserId = userId
                };
                _context.CartItems.Add(newItem);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Item added to cart." });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateQuantity(int id, [FromBody] int quantity)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item == null)
                return NotFound();

            item.Quantity = quantity;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Quantity updated." });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCartItem(int id)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Item removed from cart." });
        }
    }
}