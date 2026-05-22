using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Trabajo_Practoco_Proyecto_de_Software.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly IGetSeatBySectorIdQueryHandler _getSeatsHandler;

        public SeatController(IGetSeatBySectorIdQueryHandler getSeatsHandler)
        {
            _getSeatsHandler = getSeatsHandler;
        }


        [HttpGet("sector/{sectorId}")]
        public async Task<ActionResult<List<SeatResponseDto>>> GetBySector(int sectorId)
        {
            var seats = await _getSeatsHandler.GetSeatBySectorId(sectorId);
            return Ok(seats);
        }
    }
}
