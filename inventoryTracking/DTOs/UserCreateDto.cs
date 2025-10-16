using EnvanterBackend.Entities.Enums;

namespace EnvanterBackend.DTOs
{
    public class UserCreateDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Viewer;
        public bool CanInventory { get; set; }
        public bool CanLogs { get; set; }
        public bool CanUsers { get; set; }
    }
}
