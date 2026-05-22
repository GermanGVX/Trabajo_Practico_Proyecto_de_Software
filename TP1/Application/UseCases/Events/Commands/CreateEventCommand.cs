namespace Application.UseCases.Events.Commands
{
    public class CreateEventCommand
    {
        public string Name { get; set; }

        public DateTime EventDate { get; set; }
        public string Venue { get; set; }
    }
}
