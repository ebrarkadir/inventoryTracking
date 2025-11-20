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

        // 📌 TÜM KULLANICILAR (aktif/pasif filtreli)
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? filter = null,
            [FromQuery] bool includeInactive = false)
        {
            if (!HasUserPermission() && User.FindFirst(ClaimTypes.Role)?.Value != "Admin")
                return Forbid();

            if (string.IsNullOrEmpty(filter))
                filter = includeInactive ? "all" : "active";

            IQueryable<User> query = _context.Users.AsNoTracking();

            query = filter.ToLower() switch
            {
                "inactive" => query.Where(u => !u.IsActive),
                "all" => query,
                _ => query.Where(u => u.IsActive)
            };

            var users = await query
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    Role = u.Role.ToString(),
                    u.CanInventory,
                    u.CanLogs,
                    u.CanUsers,
                    u.CreatedAt,
                    u.IsActive
                })
                .ToListAsync();

            var username = User.Identity?.Name ?? "Anonim";
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            long.TryParse(userIdClaim, out long userId);

            await _logService.AddLogAsync(
                userName: username,
                action: "Kullanıcı Listesi Görüntülendi",
                details: $"{filter} filtresi ile {users.Count} kullanıcı listelendi.",
                entityType: "User",
                userId: userId == 0 ? null : userId
            );

            return Ok(users);
        }

        // 🆕 KULLANICI EKLEME
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
                CanUsers = dto.CanUsers,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var actorName = User.Identity?.Name ?? "Anonim";
            var actorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            long.TryParse(actorIdClaim, out long actorId);

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

        // ❌ KULLANICI PASİFLEŞTİRME (SOFT DELETE)
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

            if (!user.IsActive)
                return BadRequest("Kullanıcı zaten pasif.");

            user.IsActive = false;
            await _context.SaveChangesAsync();

            await _logService.AddLogAsync(
                userName: currentUserName,
                action: "Kullanıcı Pasifleştirildi",
                details: $"{currentUserName}, {user.Username} adlı kullanıcıyı pasif hale getirdi (Rol: {user.Role}).",
                entityType: "User",
                entityId: id,
                userId: currentUserId
            );

            return Ok(new { message = "Kullanıcı pasif hale getirildi." });
        }

        // 🔄 PASİF KULLANICILARI GERİ AKTİFLEŞTİR (RESTORE)
        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> RestoreUser(long id)
        {
            if (!HasUserPermission() && User.FindFirst(ClaimTypes.Role)?.Value != "Admin")
                return Forbid();

            var currentUserId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentUserName = User.Identity?.Name ?? "Anonim";

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            if (user.IsActive)
                return BadRequest("Kullanıcı zaten aktif.");

            user.IsActive = true;
            await _context.SaveChangesAsync();

            await _logService.AddLogAsync(
                userName: currentUserName,
                action: "Kullanıcı Geri Aktifleştirildi",
                details: $"{currentUserName}, {user.Username} kullanıcısını tekrar aktif hale getirdi.",
                entityType: "User",
                entityId: id,
                userId: currentUserId
            );

            return Ok(new { message = "Kullanıcı yeniden aktif hale getirildi." });
        }

        // ✏️ KULLANICI GÜNCELLEME
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
            var oldIsActive = user.IsActive;

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

                // Admin pasif/aktif değiştirebilir
                user.IsActive = dto.IsActive;
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
                details: $"{currentUserName}, {oldUsername} kullanıcısını güncelledi → Yeni ad: {user.Username}, Rol: {oldRole} → {user.Role}, Aktif: {oldIsActive} → {user.IsActive}, Yetkiler: {yetkiMetni}.",
                entityType: "User",
                entityId: id,
                userId: currentUserId
            );

            return Ok(new { message = "Kullanıcı başarıyla güncellendi." });
        }
    }
}
