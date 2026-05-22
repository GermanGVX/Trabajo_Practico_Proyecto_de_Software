namespace Application.UseCases.Reservation.Commands
{
    public class CreateReservationCommand
    {
        public Guid SeatId { get; set; }
        public int? UserId { get; set; }
    }
}
