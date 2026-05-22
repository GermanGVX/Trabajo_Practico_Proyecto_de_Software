using Application.DTOs;
using Application.UseCases.Reservation.Commands;


namespace Application.Interfaces
{
    public interface ICreateReservationCommandHandler
    {
        Task<ReservationResponseDto> CreateReservation(CreateReservationCommand command);
    }
}
