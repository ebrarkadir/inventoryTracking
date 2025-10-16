using EnvanterBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnvanterBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly LogService _logService;

        public LogsController(LogService logService)
        {
            _logService = logService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var canLogs = User.Claims.FirstOrDefault(c => c.Type == "canLogs")?.Value;

            if (role != "Admin" && canLogs != "true")
                return Forbid();

            var logs = await _logService.GetAllAsync();
            return Ok(logs);
        }

     
        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var canLogs = User.Claims.FirstOrDefault(c => c.Type == "canLogs")?.Value;

            if (role != "Admin" && canLogs != "true")
                return Forbid();

            await _logService.ClearAsync();

          
            await _logService.AddLogAsync(
                userName: User.Identity?.Name ?? "Bilinmeyen Kullanıcı",
                action: "Loglar Temizlendi",
                details: "Tüm log kayıtları sistemden silindi.",
                entityType: "Log"
            );

            return Ok(new { message = "Tüm loglar temizlendi." });
        }
    }
}
