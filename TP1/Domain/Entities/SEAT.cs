namespace Domain.Entities
{


    public class SEAT
    {
        public Guid Id { get; set; }
        public int SectorId { get; set; }
        public string RowIdentifier { get; set; }
        public int SeatNumber { get; set; }
        public string Status { get; set; }
        public int Version { get; set; }

        public SECTOR sector { get; set; } = null!;


    }
}
