namespace Domain.Entities
{
    public class SECTOR

    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }


        //El =null! es para asegurarle al compilador que EF Core va a llenar este dato cuando se cargue desde la BD
        public EVENT Events { get; set; } = null!;
        public List<SEAT> Seats { get; set; } = new List<SEAT>();


    }
}
