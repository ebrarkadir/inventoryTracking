namespace EnvanterBackend.DTOs
{
    public class LogDto
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string? EntityType { get; set; }
        public long? EntityId { get; set; }
    }
}
