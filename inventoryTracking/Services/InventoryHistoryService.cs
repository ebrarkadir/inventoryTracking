using EnvanterBackend.Data;
using EnvanterBackend.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EnvanterBackend.Services
{
    public class InventoryHistoryService
    {
        private readonly AppDbContext _context;

        public InventoryHistoryService(AppDbContext context)
        {
            _context = context;
        }


        public async Task AddHistoryAsync(Inventory item, string actionType, string? username)
        {
           
            var options = new JsonSerializerOptions
            {
                WriteIndented = false
            };

      
            string snapshotJson = JsonSerializer.Serialize(item, options);

    
            var history = new InventoryHistory
            {
                InventoryId = item.Id,
                ActionType = actionType,   
                ChangedBy = username,
                ChangedAt = DateTime.UtcNow,
                Snapshot = snapshotJson
            };

            _context.InventoryHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<InventoryHistory>> GetHistoryAsync(long inventoryId)
        {
            return await _context.InventoryHistories
                .Where(h => h.InventoryId == inventoryId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }
    }
}
