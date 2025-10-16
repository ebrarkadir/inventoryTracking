using EnvanterBackend.Entities.Enums;

namespace EnvanterBackend.DTOs
{
    public class InventoryCreateDto
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string ItemGroup { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public DateTime? StockInDate { get; set; }
        public DateTime? StockOutDate { get; set; }
        public string? Description { get; set; }
        public string? AssignedProject { get; set; }
        public string? AssignedPerson { get; set; }

        public InventoryStatus Status { get; set; } = InventoryStatus.Depoda;

        public bool IsActive { get; set; } = true;
    }

    public class InventoryUpdateDto
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string ItemGroup { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public DateTime? StockInDate { get; set; }
        public DateTime? StockOutDate { get; set; }
        public string? Description { get; set; }
        public string? AssignedProject { get; set; }
        public string? AssignedPerson { get; set; }

        public InventoryStatus Status { get; set; } = InventoryStatus.Depoda;

        public bool IsActive { get; set; } = true;
    }

    public class InventoryResponseDto
    {
        public long Id { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string ItemGroup { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public DateTime? StockInDate { get; set; }
        public DateTime? StockOutDate { get; set; }
        public string? Description { get; set; }
        public string? AssignedProject { get; set; }
        public string? AssignedPerson { get; set; }

        public InventoryStatus Status { get; set; }
        public DateTime LastActionDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
