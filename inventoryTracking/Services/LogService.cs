using EnvanterBackend.Data;
using EnvanterBackend.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnvanterBackend.Services
{
    public class LogService
    {
        private readonly AppDbContext _context;

        public LogService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Detaylı log kaydı ekler.
        /// </summary>
        /// <param name="userName">İşlemi yapan kullanıcı adı</param>
        /// <param name="action">Yapılan işlem türü (örnek: "Envanter Eklendi")</param>
        /// <param name="details">Ek açıklama (örnek: "SeriNo: ABC123")</param>
        /// <param name="entityType">İşlem yapılan varlık türü (örnek: "Inventory", "User", "Auth")</param>
        /// <param name="entityId">İşlem yapılan varlığın ID’si</param>
        /// <param name="userId">İşlemi yapan kullanıcının ID’si (isteğe bağlı)</param>
        public async Task AddLogAsync(
            string userName,
            string action,
            string? details = null,
            string? entityType = null,
            long? entityId = null,
            long? userId = null)
        {
            var log = new Log
            {
                UserName = userName,
                Action = action,
                Details = details,
                EntityType = entityType,
                EntityId = entityId,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Tüm logları en yeni kayıt en üstte olacak şekilde getirir.
        /// </summary>
        public async Task<List<Log>> GetAllAsync()
        {
            return await _context.Logs
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Tüm log kayıtlarını temizler.
        /// </summary>
        public async Task ClearAsync()
        {
            _context.Logs.RemoveRange(_context.Logs);
            await _context.SaveChangesAsync();
        }
    }
}
