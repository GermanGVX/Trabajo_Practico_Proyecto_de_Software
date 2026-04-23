using Application.Interfaces;
using Application.UseCases.Events.Commands;
using Application.UseCases.Events.Handlers;
using Application.UseCases.Sectors.Handlers;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Trabajo_Practoco_Proyecto_de_Software.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ICreateEventCommandHandler _CreateEvent;
        private readonly IGetEventByIdQueryHandler _GetEventById;
        private readonly IGetAllEventsQueryHandler _GetAllEvent;
        private readonly IGetSectorsByEventIdQueryHandler _GetSectorsByEventId;

        public EventsController(ICreateEventCommandHandler createEvent, IGetEventByIdQueryHandler getEventById, IGetAllEventsQueryHandler getAllEvent, IGetSectorsByEventIdQueryHandler getSectorsByEventId)
        {
            _CreateEvent = createEvent; 
            _GetEventById = getEventById;
            _GetAllEvent = getAllEvent;
            _GetSectorsByEventId = getSectorsByEventId;
        }

        

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventCommand command)
        {
            var eventId= await _CreateEvent.CreateEvent(command);

            return CreatedAtAction(
                    nameof(GetEventById),
                    new { id = eventId },
                    new { Id = eventId, Message = "Evento creado exitosamente" }
                );
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvent()
        {
            var result = await _GetAllEvent.GetAllEvents();
            return Ok( result );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var result = await _GetEventById.GetEventById(id);
            return Ok(result);
        }
        [HttpGet("{eventId}/sectors")]
        public async Task<IActionResult> GetSectorsByEventId(int eventId)
        {
            var result = await _GetSectorsByEventId.GetSectorByEventId(eventId);
            return Ok( result );
        }
    }
}
