using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Trabajo_Practoco_Proyecto_de_Software.Controllers
{
    /// <summary>
    /// Gestiona la consulta y visualización del mapa de butacas/asientos.
    /// </summary>
    [Tags("Asientos")]
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly IGetSeatBySectorIdQueryHandler _getSeatsHandler;

        public SeatController(IGetSeatBySectorIdQueryHandler getSeatsHandler)
        {
            _getSeatsHandler = getSeatsHandler;
        }
        /// <summary>
        /// Obtiene el listado completo de asientos para un sector específico.
        /// </summary>
        /// <param name="sectorId">El ID del sector del cual se quieren consultar los asientos.</param>
        /// <returns>Una lista de asientos con sus respectivos estados actuales (Disponible, Pendiente, Vendido).</returns>
        /// <response code="200">Lista de asientos retornada exitosamente.</response>
        /// <response code="400">El formato del ID del sector es inválido.</response>
        /// <response code="404">No se encontró el sector solicitado o no tiene asientos cargados.</response>
        [HttpGet("sector/{sectorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<List<SeatResponseDto>>> GetBySector(int sectorId)
        {
            var seats = await _getSeatsHandler.GetSeatBySectorId(sectorId);
            return Ok(seats);
        }
    }
}
