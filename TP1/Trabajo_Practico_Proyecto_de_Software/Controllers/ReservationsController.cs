using Application.Interfaces;
using Application.UseCases.Events.Commands;
using Application.UseCases.Reservation.Commands;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Trabajo_Practoco_Proyecto_de_Software.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ICreateReservationCommandHandler _CreateReservation;
        private readonly IConfirmPaymentCommandHandler _ConfirmPayment;
        private readonly ICancelReservationCommandHandler _cancelReservation;

        public ReservationsController(ICreateReservationCommandHandler createReservation, IConfirmPaymentCommandHandler confirmpayment, ICancelReservationCommandHandler cancelReservation)
        {
            _CreateReservation = createReservation;
            _ConfirmPayment = confirmpayment;
            _cancelReservation = cancelReservation;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation( CreateReservationCommand command)
        {
                var reservation = await _CreateReservation.CreateReservation(command);
                return StatusCode(201, reservation);
        }
        
        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmPayment(Guid id)
        {
           
                await _ConfirmPayment.ConfirmPayment(new ConfirmPaymentCommand { ReservationId = id });
                return Ok(new { Message = "¡Compra confirmada exitosamente!" });
            
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelReservation(Guid id)
        {
                await _cancelReservation.CancelReservation(new CancelReservationCommand { ReservationId = id });
                return Ok(new { message = "Reserva cancelada. Butaca liberada exitosamente." });
        }

    }
}
