namespace Domain.Entities
{
    public class AUDIT_LOG
    {
        public Guid Id { get; set; }
        public int? UserId { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string Details { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
