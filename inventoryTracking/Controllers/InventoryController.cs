using EnvanterBackend.Data;
using EnvanterBackend.DTOs;
using EnvanterBackend.Entities;
using EnvanterBackend.Entities.Enums;
using EnvanterBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EnvanterBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly InventoryExportService _exportService;
        private readonly LogService _logService;
        private readonly InventoryHistoryService _historyService;

        public InventoryController(
            AppDbContext context,
            InventoryExportService exportService,
            LogService logService,
            InventoryHistoryService historyService)
        {
            _context = context;
            _exportService = exportService;
            _logService = logService;
            _historyService = historyService;
        }

        private bool HasInventoryPermission()
        {
            var canInventory = User.Claims.FirstOrDefault(c => c.Type == "canInventory")?.Value;
            return canInventory == "true";
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return null;
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        // 📦 TÜM ENVANTERLER
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryResponseDto>>> GetAll([FromQuery] string? filter = null, [FromQuery] bool includeInactive = false)
        {
            if (!HasInventoryPermission())
                return Forbid("Envanter yetkiniz yok.");

            if (string.IsNullOrEmpty(filter))
                filter = includeInactive ? "all" : "active";

            IQueryable<Inventory> query = _context.Inventories.AsNoTracking();
            query = filter.ToLower() switch
            {
                "inactive" => query.Where(i => !i.IsActive),
                "all" => query,
                _ => query.Where(i => i.IsActive)
            };

            var items = await query
                .Select(i => new InventoryResponseDto
                {
                    Id = i.Id,
                    SerialNumber = i.SerialNumber,
                    Brand = i.Brand,
                    ItemName = i.ItemName,
                    ItemGroup = i.ItemGroup,
                    Model = i.Model,
                    StockInDate = i.StockInDate,
                    StockOutDate = i.StockOutDate,
                    Description = i.Description,
                    AssignedProject = i.AssignedProject,
                    AssignedPerson = i.AssignedPerson,
                    Status = i.Status,
                    LastActionDate = i.LastActionDate,
                    CreatedAt = i.CreatedAt,
                    IsActive = i.IsActive
                })
                .OrderByDescending(i => i.LastActionDate)
                .ToListAsync();

            await _logService.AddLogAsync(
                userName: User.Identity?.Name ?? "Anonim",
                action: "Envanter Listelendi",
                details: $"Filtre: {filter}, Toplam {items.Count} kayıt listelendi.",
                entityType: "Inventory"
            );

            return Ok(items);
        }

        // 📤 EXCEL DIŞA AKTARIM
        [HttpGet("export-file")]
        public async Task<IActionResult> ExportToExcel([FromQuery] string filter = "active")
        {
            if (!HasInventoryPermission())
                return Forbid("Envanter yetkiniz yok.");

            var fileBytes = await _exportService.ExportToExcelAsync(filter);
            var filename = $"Envanter_{filter}_{DateTime.Now:yyyyMMddHHmm}.xlsx";

            await _logService.AddLogAsync(
                User.Identity?.Name ?? "Anonim",
                "Excel Dışa Aktarım",
                $"Filtre: {filter} için Excel dosyası oluşturuldu.",
                "Inventory"
            );

            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                filename);
        }

        // 📥 EXCEL İÇE AKTARIM
        [HttpPost("import")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (!HasInventoryPermission())
                return Forbid("Envanter yetkiniz yok.");

            if (file == null || file.Length == 0)
                return BadRequest("Dosya seçilmedi.");

            using var stream = file.OpenReadStream();
            var count = await _exportService.ImportFromExcelAsync(stream);

            await _logService.AddLogAsync(
                User.Identity?.Name ?? "Anonim",
                "Excel İçe Aktarım",
                $"{count} envanter kaydı başarıyla içe aktarıldı.",
                "Inventory"
            );

            return Ok(new { message = $"{count} kayıt içe aktarıldı." });
        }

        // 🔍 TEK ENVANTER GETİRME
        [HttpGet("{id:long}")]
        public async Task<ActionResult<InventoryResponseDto>> GetById(long id)
        {
            if (!HasInventoryPermission())
                return Forbid("Envanter yetkiniz yok.");

            var item = await _context.Inventories.FindAsync(id);
            if (item == null)
                return NotFound("Envanter bulunamadı.");

            var dto = new InventoryResponseDto
            {
                Id = item.Id,
                SerialNumber = item.SerialNumber,
                Brand = item.Brand,
                ItemName = item.ItemName,
                ItemGroup = item.ItemGroup,
                Model = item.Model,
                StockInDate = item.StockInDate,
                StockOutDate = item.StockOutDate,
                Description = item.Description,
                AssignedProject = item.AssignedProject,
                AssignedPerson = item.AssignedPerson,
                Status = item.Status,
                LastActionDate = item.LastActionDate,
                CreatedAt = item.CreatedAt,
                IsActive = item.IsActive
            };

            await _logService.AddLogAsync(
                User.Identity?.Name ?? "Anonim",
                "Envanter Görüntülendi",
                $"{item.ItemName} (Seri No: {item.SerialNumber}, ID: #{item.Id}) görüntülendi.",
                "Inventory",
                id
            );

            return Ok(dto);
        }

        // ➕ ENVANTER EKLEME
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] InventoryCreateDto dto)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            if (user.Role == UserRole.Admin || (user.Role == UserRole.Constructor && user.CanInventory))
            {
                var item = new Inventory
                {
                    SerialNumber = dto.SerialNumber,
                    Brand = dto.Brand,
                    ItemName = dto.ItemName,
                    ItemGroup = dto.ItemGroup,
                    Model = dto.Model,
                    StockInDate = dto.StockInDate,
                    StockOutDate = dto.StockOutDate,
                    Description = dto.Description,
                    AssignedProject = dto.AssignedProject,
                    AssignedPerson = dto.AssignedPerson,
                    Status = dto.Status,
                    LastActionDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Inventories.Add(item);
                await _context.SaveChangesAsync();

                await _historyService.AddHistoryAsync(item, "CREATE", user.Username);
                await _logService.AddLogAsync(
                    user.Username,
                    "Envanter Eklendi",
                    $"{item.ItemName} (Seri No: {item.SerialNumber}, ID: #{item.Id}) eklendi.",
                    "Inventory",
                    item.Id,
                    user.Id
                );

                return Ok(new { message = "Envanter başarıyla eklendi." });
            }

            return Forbid("Envanter ekleme yetkiniz yok.");
        }

        // ✏️ ENVANTER GÜNCELLEME
        [HttpPut("{id:long}")]
        public async Task<ActionResult> Update(long id, [FromBody] InventoryUpdateDto dto)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            if (user.Role == UserRole.Admin || (user.Role == UserRole.Constructor && user.CanInventory))
            {
                var item = await _context.Inventories.FindAsync(id);
                if (item == null) return NotFound("Envanter bulunamadı.");

                // 🔍 Eski değerleri sakla
                var oldValues = new
                {
                    item.SerialNumber,
                    item.Brand,
                    item.ItemName,
                    item.ItemGroup,
                    item.Model,
                    item.StockInDate,
                    item.StockOutDate,
                    item.Description,
                    item.AssignedProject,
                    item.AssignedPerson,
                    item.Status
                };

                // 📝 Güncellemeleri uygula
                item.SerialNumber = dto.SerialNumber;
                item.Brand = dto.Brand;
                item.ItemName = dto.ItemName;
                item.ItemGroup = dto.ItemGroup;
                item.Model = dto.Model;
                item.StockInDate = dto.StockInDate;
                item.StockOutDate = dto.StockOutDate;
                item.Description = dto.Description;
                item.AssignedProject = dto.AssignedProject;
                item.AssignedPerson = dto.AssignedPerson;
                item.Status = dto.Status;
                item.LastActionDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await _historyService.AddHistoryAsync(item, "UPDATE", user.Username);

                // 🔎 Farkları bul
                var changes = new List<string>();

                void Compare(string fieldName, object? oldVal, object? newVal)
                {
                    if ((oldVal == null && newVal == null) ||
                        (oldVal?.ToString() ?? "") == (newVal?.ToString() ?? ""))
                        return;

                    // Durum için isimlendirme düzeltmesi
                    if (fieldName == "Status")
                    {
                        var oldLabel = Enum.GetName(typeof(InventoryStatus), oldVal) ?? oldVal?.ToString();
                        var newLabel = Enum.GetName(typeof(InventoryStatus), newVal) ?? newVal?.ToString();
                        changes.Add($"Durum: {oldLabel} → {newLabel}");
                    }
                    else
                    {
                        changes.Add($"{fieldName}: \"{oldVal}\" → \"{newVal}\"");
                    }
                }

                Compare("Seri No", oldValues.SerialNumber, dto.SerialNumber);
                Compare("Marka", oldValues.Brand, dto.Brand);
                Compare("Malzeme Adı", oldValues.ItemName, dto.ItemName);
                Compare("Grup", oldValues.ItemGroup, dto.ItemGroup);
                Compare("Model", oldValues.Model, dto.Model);
                Compare("Açıklama", oldValues.Description, dto.Description);
                Compare("Tahsis Proje", oldValues.AssignedProject, dto.AssignedProject);
                Compare("Tahsis Kişi", oldValues.AssignedPerson, dto.AssignedPerson);
                Compare("Durum", oldValues.Status, dto.Status);

                // 💬 Log açıklaması oluştur
                string details;
                if (changes.Count == 0)
                    details = $"{item.ItemName} (Seri No: {item.SerialNumber}, ID: #{item.Id}) üzerinde değişiklik yapılmadı.";
                else
                    details = $"{item.ItemName} (Seri No: {item.SerialNumber}, ID: #{item.Id}) güncellendi:\n- {string.Join("\n- ", changes)}";

                // 📜 Log kaydı
                await _logService.AddLogAsync(
                    user.Username,
                    "Envanter Güncellendi",
                    details,
                    "Inventory",
                    id,
                    user.Id
                );

                return Ok(new { message = "Envanter güncellendi." });
            }

            return Forbid("Envanter güncelleme yetkiniz yok.");
        }


        // ❌ ENVANTER PASİFLEŞTİRME
        [HttpDelete("{id:long}")]
        public async Task<ActionResult> Delete(long id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            if (user.Role == UserRole.Admin)
            {
                var item = await _context.Inventories.FindAsync(id);
                if (item == null) return NotFound("Envanter bulunamadı.");

                item.IsActive = false;
                item.LastActionDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await _historyService.AddHistoryAsync(item, "DELETE", user.Username);

                await _logService.AddLogAsync(
                    user.Username,
                    "Envanter Pasifleştirildi",
                    $"{item.ItemName} (Seri No: {item.SerialNumber}, ID: #{item.Id}) pasif hale getirildi.",
                    "Inventory",
                    id,
                    user.Id
                );

                return Ok(new { message = "Envanter silindi (pasif hale getirildi)." });
            }

            return Forbid("Envanter silme yetkiniz yok.");
        }

        // ♻️ ENVANTER GERİ YÜKLEME
        [HttpPatch("{id:long}/restore")]
        public async Task<ActionResult> Restore(long id)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();

            if (user.Role != UserRole.Admin)
                return Forbid("Geri yükleme yetkiniz yok.");

            var item = await _context.Inventories.FindAsync(id);
            if (item == null)
                return NotFound("Envanter bulunamadı.");

            item.IsActive = true;
            item.LastActionDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _historyService.AddHistoryAsync(item, "RESTORE", user.Username);

            await _logService.AddLogAsync(
                user.Username,
                "Envanter Geri Yüklendi",
                $"{item.ItemName} (Seri No: {item.SerialNumber}, ID: #{item.Id}) yeniden aktif hale getirildi.",
                "Inventory",
                id,
                user.Id
            );

            return Ok(new { message = "Envanter geri yüklendi." });
        }

        [HttpGet("distinct/{field}")]
        public async Task<IActionResult> GetDistinctValues(string field)
        {
            if (!HasInventoryPermission())
                return Forbid("Envanter yetkiniz yok.");

            IQueryable<Inventory> query = _context.Inventories.AsNoTracking();
            List<string> values;

            switch (field.ToLower())
            {
                case "serialnumber":
                    values = await query
                        .Where(i => i.SerialNumber != null && i.SerialNumber != "")
                        .Select(i => i.SerialNumber!)
                        .Distinct()
                        .OrderBy(v => v)
                        .ToListAsync();
                    break;

                case "brand":
                    values = await query
                        .Where(i => i.Brand != null && i.Brand != "")
                        .Select(i => i.Brand!)
                        .Distinct()
                        .OrderBy(v => v)
                        .ToListAsync();
                    break;

                case "itemname":
                    values = await query
                        .Where(i => i.ItemName != null && i.ItemName != "")
                        .Select(i => i.ItemName!)
                        .Distinct()
                        .OrderBy(v => v)
                        .ToListAsync();
                    break;

                case "itemgroup":
                    values = await query
                        .Where(i => i.ItemGroup != null && i.ItemGroup != "")
                        .Select(i => i.ItemGroup!)
                        .Distinct()
                        .OrderBy(v => v)
                        .ToListAsync();
                    break;

                case "model":
                    values = await query
                        .Where(i => i.Model != null && i.Model != "")
                        .Select(i => i.Model!)
                        .Distinct()
                        .OrderBy(v => v)
                        .ToListAsync();
                    break;

                case "assignedproject":
                    values = await query
                        .Where(i => i.AssignedProject != null && i.AssignedProject != "")
                        .Select(i => i.AssignedProject!)
                        .Distinct()
                        .OrderBy(v => v)
                        .ToListAsync();
                    break;

                case "assignedperson":
                    values = await query
                        .Where(i => i.AssignedPerson != null && i.AssignedPerson != "")
                        .Select(i => i.AssignedPerson!)
                        .Distinct()
                        .OrderBy(v => v)
                        .ToListAsync();
                    break;

                case "status":
                    // Enum değerlerini string olarak döndür (örnek: "0:Depoda")
                    values = Enum.GetValues(typeof(InventoryStatus))
                        .Cast<InventoryStatus>()
                        .Select(s =>
                        {
                            var num = (int)s;
                            var label = s switch
                            {
                                InventoryStatus.Depoda => "Depoda",
                                InventoryStatus.Projede => "Projede",
                                InventoryStatus.ArizaliOnarim => "Arızalı - Onarım",
                                InventoryStatus.ArizaliKullanimDisi => "Arızalı - Kullanım Dışı",
                                InventoryStatus.StoktanCikarildi => "Stoktan Çıkarıldı",
                                _ => s.ToString()
                            };
                            return $"{num}:{label}";
                        })
                        .ToList();
                    break;

                default:
                    values = new List<string>();
                    break;
            }

            return Ok(values);
        }




        [HttpGet("{id:long}/history")]
        public async Task<ActionResult<IEnumerable<InventoryHistoryDto>>> GetHistory(long id)
        {
            var history = await _historyService.GetHistoryAsync(id);
            if (history == null || history.Count == 0)
                return Ok(new List<InventoryHistoryDto>());

            var dtoList = history.Select(h =>
            {
                Inventory? snapshot = string.IsNullOrEmpty(h.Snapshot)
                    ? null
                    : JsonSerializer.Deserialize<Inventory>(h.Snapshot, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return new InventoryHistoryDto
                {
                    Id = h.Id,
                    InventoryId = h.InventoryId,
                    ActionType = h.ActionType,
                    ChangedAt = h.ChangedAt,
                    ChangedBy = h.ChangedBy,
                    SerialNumber = snapshot?.SerialNumber,
                    Brand = snapshot?.Brand,
                    ItemName = snapshot?.ItemName,
                    ItemGroup = snapshot?.ItemGroup,
                    Model = snapshot?.Model,
                    StockInDate = snapshot?.StockInDate,
                    StockOutDate = snapshot?.StockOutDate,
                    Description = snapshot?.Description,
                    AssignedProject = snapshot?.AssignedProject,
                    AssignedPerson = snapshot?.AssignedPerson,
                    Status = snapshot?.Status.ToString()
                };
            }).ToList();

            return Ok(dtoList);
        }

        // 📤 TARİHÇE EXCEL EXPORT
        [HttpGet("{id:long}/history/export")]
        public async Task<IActionResult> ExportHistoryToExcel(long id)
        {
            if (!HasInventoryPermission())
                return Forbid("Envanter yetkiniz yok.");

            var bytes = await _exportService.ExportHistoryToExcelAsync(id);
            if (bytes == null || bytes.Length == 0)
                return NotFound("Bu envanter için tarihçe bulunamadı.");

            var item = await _context.Inventories.FindAsync(id);

            await _logService.AddLogAsync(
                User.Identity?.Name ?? "Anonim",
                "Tarihçe Excel'e Aktarıldı",
                $"{item?.ItemName} (Seri No: {item?.SerialNumber}, ID: #{id}) için tarihçe dosyası oluşturuldu.",
                "Inventory",
                id
            );

            var filename = $"Envanter_{id}_Tarihce_{DateTime.Now:yyyyMMddHHmm}.xlsx";
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                filename);
        }
    }
}
