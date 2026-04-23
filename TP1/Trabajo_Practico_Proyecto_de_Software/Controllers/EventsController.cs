using Application.Interfaces;
using Application.UseCases.Events.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Trabajo_Practoco_Proyecto_de_Software.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        //private readonly IServicesGetAll _services;
        private readonly ICreateEventCommandHandler _CreateEvent;

        public EventsController(ICreateEventCommandHandler createEvent)
        {
            _CreateEvent = createEvent; 
            //_services = services;

        }

        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    var result= _services.GetAll();
        //    return new JsonResult(result);
        //}

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
        [HttpGet("{id}")]
        public IActionResult GetEventById(int id) => Ok();
    }
}
