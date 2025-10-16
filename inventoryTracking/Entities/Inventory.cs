using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EnvanterBackend.Entities.Enums;

namespace EnvanterBackend.Entities
{
    public class Inventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required, MaxLength(100)]
        public string SerialNumber { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string ItemName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ItemGroup { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;


        public DateTime? StockInDate { get; set; }
        public DateTime? StockOutDate { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? AssignedProject { get; set; }

        [MaxLength(200)]
        public string? AssignedPerson { get; set; }

        
        [Required]
        public InventoryStatus Status { get; set; } = InventoryStatus.Depoda;

        public DateTime LastActionDate { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true; 

    }
}
