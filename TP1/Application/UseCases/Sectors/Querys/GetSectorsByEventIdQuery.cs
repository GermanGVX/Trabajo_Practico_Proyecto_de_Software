namespace Application.UseCases.Sectors.Querys
{
    public class GetSectorsByEventIdQuery
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
    }
}
