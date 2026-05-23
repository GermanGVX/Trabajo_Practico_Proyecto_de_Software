namespace Domain.Entities
{

    public class RESERVATION
    {
        public Guid Id { get; set; }
        public int? UserId { get; set; }
        public Guid SeatId { get; set; }
        public string Status { get; set; }
        public DateTime ReservedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public SEAT seat { get; set; } = null!;

        public USER user { get; set; } = null!;



    }
}
