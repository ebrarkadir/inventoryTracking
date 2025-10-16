using EnvanterBackend.Entities;
using System.ComponentModel.DataAnnotations.Schema;

public class InventoryHistory
{
    public long Id { get; set; }
    public long InventoryId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? ChangedBy { get; set; }

    public string Snapshot { get; set; } = string.Empty;

    [ForeignKey("InventoryId")]
    public Inventory Inventory { get; set; }
}
