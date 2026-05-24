using Application.Interfaces;
using Application.UseCases.Events.Querys;

namespace Application.UseCases.Events.Handlers
{
    public class GetAllEventsQueryHandler : IGetAllEventsQueryHandler
    {
        private readonly IEventRepository _query;

        public GetAllEventsQueryHandler(IEventRepository query)
        {
            _query = query;
        }

        public async Task<List<GetAllEventsQuery>> GetAllEvents()
        {
            var events = await _query.GetListEvents();

            return events.Select(e => new GetAllEventsQuery
            {
                Id = e.Id,
                Name = e.Name,
                EventDate = e.EventDate,
                Venue = e.Venue
            }).ToList();
        }
    }
}
