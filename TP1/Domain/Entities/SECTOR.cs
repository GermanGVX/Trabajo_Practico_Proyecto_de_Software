namespace Domain.Entities
{
    public class SECTOR

    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }

        public EVENT Events { get; set; } = null!;
        public List<SEAT> Seats { get; set; } = new List<SEAT>();


    }
}
