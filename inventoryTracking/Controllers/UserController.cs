using EnvanterBackend.Data;
using EnvanterBackend.DTOs;
using EnvanterBackend.Entities;
using EnvanterBackend.Entities.Enums;
using EnvanterBackend.Services;
using inventoryTracking.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EnvanterBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;

        public UserController(AppDbContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        private bool HasUserPermission()
        {
            var canUsers = User.Claims.FirstOrDefault(c => c.Type == "canUsers")?.Value;
            return canUsers == "true";
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!HasUserPermission() && User.FindFirst(ClaimTypes.Role)?.Value != "Admin")
                return Forbid();

            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    Role = u.Role.ToString(),
                    u.CanInventory,
                    u.CanLogs,
                    u.CanUsers,
                    u.CreatedAt
                })
                .ToListAsync();

            var username = User.Identity?.Name ?? "Anonim";
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            long.TryParse(userIdClaim, out long userId);

            await _logService.AddLogAsync(
                userName: username,
                action: "Kullanıcı Listesi Görüntülendi",
                details: $"{users.Count} kullanıcı listelendi.",
                entityType: "User",
                userId: userId == 0 ? null : userId
            );

            return Ok(users);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto dto)
        {
            if (!HasUserPermission() && User.FindFirst(ClaimTypes.Role)?.Value != "Admin")
                return Forbid();

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Bu kullanıcı adı zaten mevcut.");

            if (!Enum.IsDefined(typeof(UserRole), dto.Role))
                return BadRequest("Geçersiz rol değeri.");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                CanInventory = dto.CanInventory,
                CanLogs = dto.CanLogs,
                CanUsers = dto.CanUsers
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var actorName = User.Identity?.Name ?? "Anonim";
            var actorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            long.TryParse(actorIdClaim, out long actorId);

            // yetki metni oluştur
            var yetkiler = new List<string>();
            if (user.CanInventory) yetkiler.Add("Envanter");
            if (user.CanLogs) yetkiler.Add("Log");
            if (user.CanUsers) yetkiler.Add("Kullanıcı");
            var yetkiMetni = yetkiler.Any() ? string.Join(", ", yetkiler) : "Sınırlı";

            await _logService.AddLogAsync(
                userName: actorName,
                action: "Yeni Kullanıcı Eklendi",
                details: $"{actorName}, yeni kullanıcı ekledi: {user.Username} (Rol: {user.Role}, Yetkiler: {yetkiMetni}).",
                entityType: "User",
                entityId: user.Id,
                userId: actorId == 0 ? null : actorId
            );

            return Ok(new { message = "Kullanıcı başarıyla oluşturuldu." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            if (!HasUserPermission() && User.FindFirst(ClaimTypes.Role)?.Value != "Admin")
                return Forbid();

            var currentUserId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentUserName = User.Identity?.Name ?? "Anonim";

            if (currentUserId == id)
                return BadRequest("Kendi hesabınızı silemezsiniz.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            await _logService.AddLogAsync(
                userName: currentUserName,
                action: "Kullanıcı Silindi",
                details: $"{currentUserName}, {user.Username} adlı kullanıcıyı sildi (Rol: {user.Role}).",
                entityType: "User",
                entityId: id,
                userId: currentUserId
            );

            return Ok(new { message = "Kullanıcı başarıyla silindi." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UserUpdateDto dto)
        {
            var currentUserId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentUserRole = User.FindFirst(ClaimTypes.Role)!.Value;
            var currentUserName = User.Identity?.Name ?? "Anonim";

            if (currentUserRole != "Admin" && currentUserId != id)
                return Forbid();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            var oldRole = user.Role;
            var oldUsername = user.Username;

            user.Username = dto.Username;

            if (!string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            if (currentUserRole == "Admin" || HasUserPermission())
            {
                if (!Enum.IsDefined(typeof(UserRole), dto.Role))
                    return BadRequest("Geçersiz rol değeri.");

                user.Role = dto.Role;
                user.CanInventory = dto.CanInventory;
                user.CanLogs = dto.CanLogs;
                user.CanUsers = dto.CanUsers;
            }

            await _context.SaveChangesAsync();

            var yetkiler = new List<string>();
            if (user.CanInventory) yetkiler.Add("Envanter");
            if (user.CanLogs) yetkiler.Add("Log");
            if (user.CanUsers) yetkiler.Add("Kullanıcı");
            var yetkiMetni = yetkiler.Any() ? string.Join(", ", yetkiler) : "Sınırlı";

            await _logService.AddLogAsync(
                userName: currentUserName,
                action: "Kullanıcı Güncellendi",
                details: $"{currentUserName}, {oldUsername} kullanıcısını güncelledi → Yeni ad: {user.Username}, Rol: {oldRole} → {user.Role}, Yetkiler: {yetkiMetni}.",
                entityType: "User",
                entityId: id,
                userId: currentUserId
            );

            return Ok(new { message = "Kullanıcı başarıyla güncellendi." });
        }
    }
}
