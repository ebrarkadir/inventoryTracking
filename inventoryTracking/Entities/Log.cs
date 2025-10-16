namespace EnvanterBackend.Entities
{
    public class Log
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserName { get; set; } = string.Empty;  
        public string Action { get; set; } = string.Empty;     
        public string? Details { get; set; }                   
        public string? EntityType { get; set; }                
        public long? EntityId { get; set; }                    

        // Kullanıcı referansı
        public long? UserId { get; set; }
        public User? User { get; set; }
    }
}
