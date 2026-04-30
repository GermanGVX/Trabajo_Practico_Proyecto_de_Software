using System.Reflection.Metadata;
using Application.Interfaces;
using Application.UseCases.Events.Commands;
using Application.UseCases.Events.Handlers;
using Application.UseCases.Sectors.Handlers;
<<<<<<< HEAD
using Domain.Entities;
=======
using Domain.Exceptions;
>>>>>>> f326cc82e92634ce93fd468514a8591c3af94a97
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
<<<<<<< HEAD
        private readonly IGetSeatBySectorIdQueryHandler _GetSeatsBySectorId;

        public EventsController(ICreateEventCommandHandler createEvent, IGetEventByIdQueryHandler getEventById, IGetAllEventsQueryHandler getAllEvent, IGetSectorsByEventIdQueryHandler getSectorsByEventId, IGetSeatBySectorIdQueryHandler getSeatsBySectorId)
        {
            _CreateEvent = createEvent;
            _GetEventById = getEventById;
            _GetAllEvent = getAllEvent;
            _GetSectorsByEventId = getSectorsByEventId;
            _GetSeatsBySectorId = getSeatsBySectorId;
=======
        private readonly ICreateReservationCommandHandler _CreateReservation;

        public EventsController(ICreateEventCommandHandler createEvent, IGetEventByIdQueryHandler getEventById, IGetAllEventsQueryHandler getAllEvent, IGetSectorsByEventIdQueryHandler getSectorsByEventId, ICreateReservationCommandHandler createReservation)
        {
            _CreateEvent = createEvent; 
            _GetEventById = getEventById;
            _GetAllEvent = getAllEvent;
            _GetSectorsByEventId = getSectorsByEventId;
            _CreateReservation = createReservation;
>>>>>>> f326cc82e92634ce93fd468514a8591c3af94a97
        }
       

<<<<<<< HEAD

=======
>>>>>>> f326cc82e92634ce93fd468514a8591c3af94a97

        [HttpPost]
        public async Task<IActionResult> CreateEvent( CreateEventCommand command)
        {
            var eventId = await _CreateEvent.CreateEvent(command);

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
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var result = await _GetEventById.GetEventById(id);
            return Ok(result);
        }
<<<<<<< HEAD
=======

        [HttpGet]
        public async Task<IActionResult> GetAllEvent()
        {
            var result = await _GetAllEvent.GetAllEvents();
            return Ok( result );
        }

        
>>>>>>> f326cc82e92634ce93fd468514a8591c3af94a97
        [HttpGet("{eventId}/sectors")]
        public async Task<IActionResult> GetSectorsByEventId(int eventId)
        {
            var result = await _GetSectorsByEventId.GetSectorByEventId(eventId);
<<<<<<< HEAD
            return Ok(result);
        }

        [HttpGet("{sectorId}/seats")]

            public async Task<IActionResult> GetSeatsBySectorId(int sectorId)
            {
                var result = await _GetSeatsBySectorId.GetSeatBySectorId(sectorId);
            return Ok(result);
        }
=======
            return Ok( result );
        }


>>>>>>> f326cc82e92634ce93fd468514a8591c3af94a97
    }
}
