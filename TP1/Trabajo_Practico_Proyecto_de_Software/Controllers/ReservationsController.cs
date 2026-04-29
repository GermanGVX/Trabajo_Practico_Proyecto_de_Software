using Application.Interfaces;
using Application.UseCases.Events.Commands;
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

        public ReservationsController(ICreateReservationCommandHandler createReservation)
        {
            _CreateReservation = createReservation;
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
                return NotFound(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (ConflictException ex)
            {
                return StatusCode(409, new { Error = ex.Message });
            }
        }
    }
}
