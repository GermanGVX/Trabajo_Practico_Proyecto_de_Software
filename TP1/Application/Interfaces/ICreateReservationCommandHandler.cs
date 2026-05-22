using Application.DTOs;
using Application.UseCases.Events.Commands;


namespace Application.Interfaces
{
    public interface ICreateReservationCommandHandler
    {
        Task<ReservationResponseDto> CreateReservation(CreateReservationCommand command);
    }
}
