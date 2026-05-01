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
            try
            {
                var reservation = await _CreateReservation.CreateReservation(command);
                return StatusCode(201, reservation);


            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (ConflictException ex)
            {
                return StatusCode(409, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"❌❌❌ ERROR CRÍTICO EN RESERVA ❌❌❌");
                Console.WriteLine($"Tipo: {ex.GetType().Name}");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Type: {ex.InnerException.GetType().Name}");
                    Console.WriteLine($"Inner Message: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Stack: {ex.InnerException.StackTrace}");
                }

                return StatusCode(500, new
                {
                    error = "Error interno del servidor",
                    
                    debug = ex.Message
                });
            }
        }
        // POST /api/reservations/{id}/confirm
        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmPayment(Guid id)
        {
            try
            {
                await _ConfirmPayment.ConfirmPayment(new ConfirmPaymentCommand { ReservationId = id });
                return Ok(new { Message = "¡Compra confirmada exitosamente!" });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { Error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { Error = ex.Message }); }
            catch (ConflictException ex) { return StatusCode(409, new { Error = ex.Message }); }

        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelReservation(Guid id)
        {
            try
            {
                await _cancelReservation.CancelReservation(new CancelReservationCommand { ReservationId = id });
                return Ok(new { message = "Reserva cancelada. Butaca liberada exitosamente." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR CANCELANDO: {ex.Message}");
                return StatusCode(500, new { error = "Error interno al procesar la cancelación." });
            }
        }

    }
}
