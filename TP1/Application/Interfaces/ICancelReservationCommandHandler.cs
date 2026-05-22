using Application.UseCases.Reservation.Commands;

namespace Application.Interfaces
{
    public interface ICancelReservationCommandHandler
    {
        Task CancelReservation(CancelReservationCommand command);
    }
}
