using Application.Interfaces;
using Application.UseCases.Events.Commands;
using Application.UseCases.Reservation.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Trabajo_Practoco_Proyecto_de_Software.Controllers
{
    /// <summary>
    /// Gestiona todas las operaciones relacionadas con la reserva, confirmación y cancelación de butacas.
    /// </summary>
    [Tags("Reservas")]
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

        /// <summary>
        /// Crea una nueva reserva de butaca (Estado Pending).
        /// </summary>
        /// <remarks>
        /// Este endpoint bloquea la butaca temporalmente por 5 minutos hasta que se confirme el pago.
        /// </remarks>
        /// <param name="command">Datos requeridos para generar la reserva.</param>
        /// <returns>La reserva generada con sus detalles.</returns>
        /// <response code="201">Reserva creada exitosamente.</response>
        /// <response code="400">Error de validación, la butaca no existe o ya está ocupada.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateReservation(CreateReservationCommand command)
        {
            var reservation = await _CreateReservation.CreateReservation(command);
            return StatusCode(201, reservation);
        }

        /// <summary>
        /// Confirma el pago de una reserva pendiente, cambiando su estado a Vendida (Sold).
        /// </summary>
        /// <param name="id">El ID único de la reserva a confirmar.</param>
        /// <returns>Un mensaje indicando el éxito de la operación.</returns>
        /// <response code="200">Pago confirmado y butaca vendida exitosamente.</response>
        /// <response code="400">La reserva ya expiró, ya está pagada o hay un error de negocio.</response>
        /// <response code="404">No se encontró una reserva con ese ID.</response>
        [HttpPost("{id}/confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> ConfirmPayment(Guid id)
        {
            await _ConfirmPayment.ConfirmPayment(new ConfirmPaymentCommand { ReservationId = id });
            return Ok(new { Message = "¡Compra confirmada exitosamente!" });
        }

        /// <summary>
        /// Cancela de forma manual una reserva y libera la butaca al instante.
        /// </summary>
        /// <param name="id">El ID único de la reserva a cancelar.</param>
        /// <returns>Un mensaje indicando que la butaca fue liberada.</returns>
        /// <response code="200">Reserva cancelada exitosamente.</response>
        /// <response code="400">La reserva no se puede cancelar (ej. ya fue vendida).</response>
        /// <response code="404">No se encontró una reserva con ese ID.</response>
        [HttpPost("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CancelReservation(Guid id)
        {
            await _cancelReservation.CancelReservation(new CancelReservationCommand { ReservationId = id });
            return Ok(new { message = "Reserva cancelada. Butaca liberada exitosamente." });
        }
    }
}
