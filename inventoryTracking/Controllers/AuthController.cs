using EnvanterBackend.Data;
using EnvanterBackend.DTOs;
using EnvanterBackend.Entities;
using EnvanterBackend.Services;
using inventoryTracking.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EnvanterBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly LogService _logService;

        public AuthController(AppDbContext context, IConfiguration config, LogService logService)
        {
            _context = context;
            _config = config;
            _logService = logService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");

            var accessToken = GenerateJwtToken(user);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString("N"),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            // Yetkileri yazıya çevir
            var yetkiler = new List<string>();
            if (user.CanInventory) yetkiler.Add("Envanter");
            if (user.CanLogs) yetkiler.Add("Log");
            if (user.CanUsers) yetkiler.Add("Kullanıcı Yönetimi");
            var yetkiMetni = yetkiler.Any() ? string.Join(", ", yetkiler) : "Sınırlı";

            await _logService.AddLogAsync(
                userName: user.Username,
                action: "Sisteme Giriş",
                details: $"{user.Username} sisteme giriş yaptı (Rol: {user.Role}, Yetkiler: {yetkiMetni}).",
                entityType: "Auth",
                userId: user.Id
            );

            return Ok(new
            {
                token = accessToken,
                refreshToken = refreshToken.Token,
                username = user.Username,
                role = user.Role.ToString(),
                canInventory = user.CanInventory,
                canLogs = user.CanLogs,
                canUsers = user.CanUsers
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name ?? "Bilinmeyen Kullanıcı";
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            long.TryParse(userIdClaim, out long userId);

            await _logService.AddLogAsync(
                userName: username,
                action: "Sistemden Çıkış",
                details: $"{username} sistemden çıkış yaptı.",
                entityType: "Auth",
                userId: userId == 0 ? null : userId
            );

            return Ok(new { message = "Çıkış işlemi kaydedildi." });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("canInventory", user.CanInventory.ToString().ToLower()),
                new Claim("canLogs", user.CanLogs.ToString().ToLower()),
                new Claim("canUsers", user.CanUsers.ToString().ToLower())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
