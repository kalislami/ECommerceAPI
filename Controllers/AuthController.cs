using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceApi.Data;
using ECommerceApi.Models;
using ECommerceApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using ECommerceApi.Helpers;

namespace ECommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthController(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(registerRequest.Email))
                    return BadRequest("Email is required.");

                var emailExists = await _context.Users.AnyAsync(u => u.Email == registerRequest.Email);
                if (emailExists)
                    return BadRequest("Email already exists.");

                string username = string.IsNullOrEmpty(registerRequest.Username) ?
                    GenerateUsernameFromEmail(registerRequest.Email) :
                    registerRequest.Username;

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

                var user = new User
                {
                    Username = username,
                    Email = registerRequest.Email,
                    Password = hashedPassword,
                    Role = registerRequest.Role
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register Error Message: {ex.Message}");
                return BadRequest("Ada yang salah");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == loginRequest.UsernameOrEmail || u.Email == loginRequest.UsernameOrEmail);

            if (user == null)
                return Unauthorized("Invalid credentials.");

            
            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
                return Unauthorized("Invalid credentials.");

            var existingTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == user.Id)
                .ToListAsync();

            if (existingTokens.Any())
                _context.RefreshTokens.RemoveRange(existingTokens);

            var accessToken = _jwtHelper.GenerateAccessToken(user);
            var refreshToken = _jwtHelper.GenerateRefreshToken();

            refreshToken.UserId = user.Id;
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == tokenRequest.refreshToken);

            if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("Refresh token tidak valid atau sudah kadaluarsa.");

            var user = await _context.Users.FindAsync(refreshToken.UserId);
            if (user == null)
                return Unauthorized("Refresh token tidak valid atau sudah kadaluarsa.");

            var newAccessToken = _jwtHelper.GenerateAccessToken(user);
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();
            newRefreshToken.UserId = user.Id;

            _context.RefreshTokens.Remove(refreshToken);
            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] TokenRequest tokenRequest)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == tokenRequest.refreshToken);
            if (token != null)
            {
                _context.RefreshTokens.Remove(token);
                await _context.SaveChangesAsync();
            }

            return Ok("Logout berhasil, refresh token telah dihapus.");
        }

        // Fungsi untuk generate username berdasarkan email (contoh: ambil sebelum tanda '@')
        static string GenerateUsernameFromEmail(string email)
        {
            return email.Split('@')[0];
        }
    }
}