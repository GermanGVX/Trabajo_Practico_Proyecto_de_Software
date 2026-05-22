namespace Application.UseCases.Events.Commands
{
    public class CreateReservationCommand
    {
        public Guid SeatId { get; set; }
        public int? UserId { get; set; }
    }
}
