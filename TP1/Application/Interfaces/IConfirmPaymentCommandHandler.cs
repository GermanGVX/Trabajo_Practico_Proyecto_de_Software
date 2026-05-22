using Application.UseCases.Reservation.Commands;

namespace Application.Interfaces
{
    public interface IConfirmPaymentCommandHandler
    {
        Task ConfirmPayment(ConfirmPaymentCommand command);
    }
}
