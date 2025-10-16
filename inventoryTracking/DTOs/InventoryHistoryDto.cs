public class InventoryHistoryDto
{
    public long Id { get; set; }
    public long InventoryId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string? ChangedBy { get; set; }

    public string? SerialNumber { get; set; }
    public string? Brand { get; set; }
    public string? ItemName { get; set; }
    public string? ItemGroup { get; set; }
    public string? Model { get; set; }
    public DateTime? StockInDate { get; set; }
    public DateTime? StockOutDate { get; set; }
    public string? Description { get; set; }
    public string? AssignedProject { get; set; }
    public string? AssignedPerson { get; set; }

    public string? Status { get; set; }
}
