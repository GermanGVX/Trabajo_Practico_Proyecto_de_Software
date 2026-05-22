namespace Domain.Entities
{
    public class EVENT
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EventDate { get; set; }
        public string Venue { get; set; }
        public string Status { get; set; } = "Active";

        public List<SECTOR> sectors { get; set; } = new List<SECTOR>();
    }
}
