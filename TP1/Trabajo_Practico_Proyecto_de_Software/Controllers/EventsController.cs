using Application.Interfaces;
using Application.UseCases.Events.Commands;
using Application.UseCases.Events.Querys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Trabajo_Practoco_Proyecto_de_Software.Controllers
{

    /// <summary>
    /// Gestiona la creación y consulta de espectáculos/eventos y sus respectivos sectores.
    /// </summary>
    [Tags("Eventos")]
    [Route("api/v1/[controller]")]
    [ApiController]

    public class EventsController : ControllerBase
    {
        private readonly ICreateEventCommandHandler _CreateEvent;
        private readonly IGetEventByIdQueryHandler _GetEventById;
        private readonly IGetPagedEventsHandler _GetPagedEvents;
        private readonly IGetSectorsByEventIdQueryHandler _GetSectorsByEventId;
        private readonly ICreateReservationCommandHandler _CreateReservation;
        private readonly IGetSeatBySectorIdQueryHandler _GetSeatsBySectorId;

        public EventsController(ICreateEventCommandHandler createEvent, IGetEventByIdQueryHandler getEventById, IGetPagedEventsHandler getPagedEvents, IGetSectorsByEventIdQueryHandler getSectorsByEventId, IGetSeatBySectorIdQueryHandler getSeatsBySectorId, ICreateReservationCommandHandler createReservation)
        {
            _CreateEvent = createEvent;
            _GetEventById = getEventById;
            _GetPagedEvents = getPagedEvents;
            _GetSectorsByEventId = getSectorsByEventId;
            _GetSeatsBySectorId = getSeatsBySectorId;
            _CreateReservation = createReservation;
        }

        /// <summary>
        /// Crea un nuevo evento en el sistema.
        /// </summary>
        /// <param name="command">Datos requeridos (Nombre, Fecha, etc.) para crear el evento.</param>
        /// <returns>La ruta para consultar el evento recién creado.</returns>
        /// <response code="201">Evento creado exitosamente.</response>
        /// <response code="400">Error de validación en los datos enviados.</response>
        [HttpPost] // <-- Solo un HttpPost
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateEvent(CreateEventCommand command)
        {
            var eventId = await _CreateEvent.CreateEvent(command);

            return CreatedAtAction(
                    nameof(GetEventById),
                    new { id = eventId },
                    new { Id = eventId, Message = "Evento creado exitosamente" }
                );
        }

        /// <summary>
        /// Obtiene el listado paginado de los eventos disponibles.
        /// </summary>
        /// <param name="query">Parámetros de paginación (ej: PageNumber y PageSize).</param>
        /// <returns>Una lista paginada de eventos.</returns>
        /// <response code="200">Lista de eventos retornada exitosamente.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllEvent([FromQuery] GetPagedEventsQuery query)
        {
            // Le pasamos el query con los parámetros (PageNumber, PageSize) a tu nuevo handler
            var result = await _GetPagedEvents.GetPagedEvents(query);

            return Ok(result);
        }

        /// <summary>
        /// Obtiene los detalles de un evento específico según su ID.
        /// </summary>
        /// <param name="id">El ID único del evento.</param>
        /// <returns>La información detallada del evento.</returns>
        /// <response code="200">Evento encontrado exitosamente.</response>
        /// <response code="404">No se encontró ningún evento con el ID proporcionado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetEventById(int id)
        {
            var result = await _GetEventById.GetEventById(id);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene todos los sectores (y sus precios) asociados a un evento específico.
        /// </summary>
        /// <param name="eventId">El ID del evento del cual se quieren consultar los sectores.</param>
        /// <returns>Una lista de sectores vinculados al evento.</returns>
        /// <response code="200">Lista de sectores retornada exitosamente.</response>
        /// <response code="404">No se encontró el evento o no tiene sectores asociados.</response>
        [HttpGet("{eventId}/sectors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetSectorsByEventId(int eventId)
        {
            var result = await _GetSectorsByEventId.GetSectorByEventId(eventId);
            return Ok(result);
        }
    }
}
