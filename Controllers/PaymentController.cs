using Microsoft.AspNetCore.Mvc;
using ECommerceApi.DTOs;
using ECommerceApi.Services;
using Microsoft.AspNetCore.Authorization;
using ECommerceApi.Data;
using System.Security.Claims;

namespace ECommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly MidtransService _midtransService;
        private readonly AppDbContext _context;

        public PaymentController(MidtransService midtransService, AppDbContext context)
        {
            _midtransService = midtransService;
            _context = context;
        }

        private string? GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("User ID tidak ditemukan dalam token.");

            var response = await _midtransService.CreateTransactionAsync(request);
            if (response == null)
                return BadRequest("Gagal membuat transaksi.");

            var order = await _context.Orders.FindAsync(request.OrderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = "Pending Payment";
            await _context.SaveChangesAsync();

            return Ok(response);
        }

        [HttpPost("update-status")]
        public async Task<IActionResult> UpdatePaymentStatus([FromBody] OrderStatusRequest request)
        {
            var order = await _context.Orders.FindAsync(request.OrderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = request.Status;
            await _context.SaveChangesAsync();

            return Ok("status order updated");
        }
    }
}