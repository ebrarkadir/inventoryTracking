using EnvanterBackend.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace EnvanterBackend.Entities
{
    public class User
    {
        public long Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } = UserRole.Viewer;

        public bool CanInventory { get; set; } = false;
        public bool CanUsers { get; set; } = false;
        public bool CanLogs { get; set; } = false;
        public ICollection<Log>? Logs { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
    }
}
