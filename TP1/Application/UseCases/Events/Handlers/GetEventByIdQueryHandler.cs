using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Application.Interfaces;
using Application.UseCases.Events.Querys;

namespace Application.UseCases.Events.Handlers
{
    public class GetEventByIdQueryHandler : IGetEventByIdQueryHandler
    {
        private readonly IEventRepository _query;

        public GetEventByIdQueryHandler(IEventRepository query)
        {
            _query = query;
        }

        public Task<GetEventByIdQuery> GetEventById(int eventId)
        {
            var events = _query.GetEvent(eventId);
            if (events   == null)
            {
                throw new KeyNotFoundException($"Evento con ID {eventId} no encontrado");
            }
            return Task.FromResult(new GetEventByIdQuery
            {
                Name = events.Name,
                Venue = events.Venue,
                EventDate = events.EventDate,
            });
        }
    }
}
