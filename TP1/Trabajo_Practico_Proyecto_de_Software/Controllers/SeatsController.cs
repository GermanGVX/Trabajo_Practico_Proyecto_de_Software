using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Trabajo_Practoco_Proyecto_de_Software.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly IGetSeatBySectorIdQueryHandler _getSeatsHandler;

        public SeatsController(IGetSeatBySectorIdQueryHandler getSeatsHandler)
        {
            _getSeatsHandler = getSeatsHandler;
        }


        [HttpGet("/api/v1/sectors/{sectorId}/seats")]
        public async Task<ActionResult<List<SeatResponseDto>>> GetBySector(int sectorId)
        {
            var seats = await _getSeatsHandler.GetSeatBySectorId(sectorId);
            return Ok(seats);
        }
    }
}
