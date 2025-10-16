using ClosedXML.Excel;
using EnvanterBackend.Data;
using EnvanterBackend.Entities;
using EnvanterBackend.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EnvanterBackend.Services
{
    public class InventoryExportService
    {
        private readonly AppDbContext _context;

        public InventoryExportService(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<byte[]> ExportToExcelAsync(string filter = "active")
        {
            IQueryable<Inventory> query = _context.Inventories.AsQueryable();

            switch (filter.ToLower())
            {
                case "inactive":
                    query = query.Where(i => !i.IsActive);
                    break;
                case "all":
                    break;
                default:
                    query = query.Where(i => i.IsActive);
                    break;
            }

            var list = await query
                .OrderByDescending(i => i.LastActionDate)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Envanter Listesi");

            
            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Seri No";
            ws.Cell(1, 3).Value = "Marka";
            ws.Cell(1, 4).Value = "Malzeme Adı";
            ws.Cell(1, 5).Value = "Malzeme Grubu";
            ws.Cell(1, 6).Value = "Model";
            ws.Cell(1, 7).Value = "Stok Giriş";
            ws.Cell(1, 8).Value = "Stok Çıkış";
            ws.Cell(1, 9).Value = "Açıklama";
            ws.Cell(1, 10).Value = "Tahsis Edilen Proje";
            ws.Cell(1, 11).Value = "Tahsis Edilen Kişi";
            ws.Cell(1, 12).Value = "Durum";
            ws.Cell(1, 13).Value = "Oluşturulma";
            ws.Cell(1, 14).Value = "Son İşlem";
            ws.Cell(1, 15).Value = "Aktif mi?";

            int row = 2;
            foreach (var i in list)
            {
                ws.Cell(row, 1).Value = i.Id;
                ws.Cell(row, 2).Value = i.SerialNumber;
                ws.Cell(row, 3).Value = i.Brand;
                ws.Cell(row, 4).Value = i.ItemName;
                ws.Cell(row, 5).Value = i.ItemGroup;
                ws.Cell(row, 6).Value = i.Model;
                ws.Cell(row, 7).Value = FormatDate(i.StockInDate);
                ws.Cell(row, 8).Value = FormatDate(i.StockOutDate);
                ws.Cell(row, 9).Value = i.Description;
                ws.Cell(row, 10).Value = i.AssignedProject;
                ws.Cell(row, 11).Value = i.AssignedPerson;
                ws.Cell(row, 12).Value = GetStatusLabel(i.Status);
                ws.Cell(row, 13).Value = FormatDateTime(i.CreatedAt);
                ws.Cell(row, 14).Value = FormatDateTime(i.LastActionDate);
                ws.Cell(row, 15).Value = i.IsActive ? "Aktif" : "Pasif";
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        
        public async Task<int> ImportFromExcelAsync(Stream fileStream)
        {
            using var workbook = new XLWorkbook(fileStream);
            var ws = workbook.Worksheets.First();

            int rowCount = 0;
            foreach (var row in ws.RowsUsed().Skip(1)) 
            {
                var serial = row.Cell(2).GetString();
                if (string.IsNullOrWhiteSpace(serial))
                    continue;

                var inv = new Inventory
                {
                    SerialNumber = serial,
                    Brand = row.Cell(3).GetString(),
                    ItemName = row.Cell(4).GetString(),
                    ItemGroup = row.Cell(5).GetString(),
                    Model = row.Cell(6).GetString(),
                    StockInDate = TryParseDate(row.Cell(7).GetString()),
                    StockOutDate = TryParseDate(row.Cell(8).GetString()),
                    Description = row.Cell(9).GetString(),
                    AssignedProject = row.Cell(10).GetString(),
                    AssignedPerson = row.Cell(11).GetString(),
                    Status = ParseStatus(row.Cell(12).GetString()),
                    CreatedAt = DateTime.UtcNow,
                    LastActionDate = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Inventories.Add(inv);
                rowCount++;
            }

            await _context.SaveChangesAsync();
            return rowCount;
        }

        
        private static string FormatDate(object? date)
        {
            if (date == null) return "";

            if (date is DateTime dt)
                return dt.ToString("yyyy-MM-dd");

            if (date is DateOnly dOnly)
                return dOnly.ToString("yyyy-MM-dd");

            return date.ToString() ?? "";
        }

        private static string FormatDateTime(object? date)
        {
            if (date == null) return "";

            if (date is DateTime dt)
                return dt.ToString("yyyy-MM-dd HH:mm");

            if (date is DateOnly dOnly)
                return dOnly.ToString("yyyy-MM-dd");

            return date.ToString() ?? "";
        }

        private static DateTime? TryParseDate(string value)
        {
            if (DateTime.TryParse(value, out var d))
                return d;
            return null;
        }

        private static InventoryStatus ParseStatus(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return InventoryStatus.Depoda;

            text = text.Trim().ToLower();

            if (text.Contains("projede")) return InventoryStatus.Projede;
            if (text.Contains("onarım")) return InventoryStatus.ArizaliOnarim;
            if (text.Contains("kullanım dışı")) return InventoryStatus.ArizaliKullanimDisi;
            if (text.Contains("stoktan")) return InventoryStatus.StoktanCikarildi;

            return InventoryStatus.Depoda;
        }

        private static string GetStatusLabel(InventoryStatus status)
        {
            return status switch
            {
                InventoryStatus.Depoda => "Depoda",
                InventoryStatus.Projede => "Projede",
                InventoryStatus.ArizaliOnarim => "Arızalı - Onarım",
                InventoryStatus.ArizaliKullanimDisi => "Arızalı - Kullanım Dışı",
                InventoryStatus.StoktanCikarildi => "Stoktan Çıkarıldı",
                _ => ""
            };
        }

        
        public async Task<byte[]> ExportHistoryToExcelAsync(long inventoryId)
        {
            var historyList = await _context.InventoryHistories
                .Where(h => h.InventoryId == inventoryId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Tarihçe");

            
            ws.Cell(1, 1).Value = "İşlem";
            ws.Cell(1, 2).Value = "Kullanıcı";
            ws.Cell(1, 3).Value = "Tarih";
            ws.Cell(1, 4).Value = "Seri No";
            ws.Cell(1, 5).Value = "Marka";
            ws.Cell(1, 6).Value = "Malzeme Adı";
            ws.Cell(1, 7).Value = "Grup";
            ws.Cell(1, 8).Value = "Model";
            ws.Cell(1, 9).Value = "Durum";
            ws.Cell(1, 10).Value = "Stok Giriş";
            ws.Cell(1, 11).Value = "Stok Çıkış";
            ws.Cell(1, 12).Value = "Açıklama";
            ws.Cell(1, 13).Value = "Tahsis Proje";
            ws.Cell(1, 14).Value = "Tahsis Kişi";

            int row = 2;
            foreach (var h in historyList)
            {
                Inventory? snapshot = string.IsNullOrEmpty(h.Snapshot)
                    ? null
                    : JsonSerializer.Deserialize<Inventory>(h.Snapshot, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                ws.Cell(row, 1).Value = h.ActionType;
                ws.Cell(row, 2).Value = h.ChangedBy;
                ws.Cell(row, 3).Value = h.ChangedAt.ToString("yyyy-MM-dd HH:mm");
                ws.Cell(row, 4).Value = snapshot?.SerialNumber ?? "";
                ws.Cell(row, 5).Value = snapshot?.Brand ?? "";
                ws.Cell(row, 6).Value = snapshot?.ItemName ?? "";
                ws.Cell(row, 7).Value = snapshot?.ItemGroup ?? "";
                ws.Cell(row, 8).Value = snapshot?.Model ?? "";
                ws.Cell(row, 9).Value = snapshot != null ? GetStatusLabel(snapshot.Status) : "";
                ws.Cell(row, 10).Value = FormatDate(snapshot?.StockInDate);
                ws.Cell(row, 11).Value = FormatDate(snapshot?.StockOutDate);
                ws.Cell(row, 12).Value = snapshot?.Description ?? "";
                ws.Cell(row, 13).Value = snapshot?.AssignedProject ?? "";
                ws.Cell(row, 14).Value = snapshot?.AssignedPerson ?? "";
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

    }
}
