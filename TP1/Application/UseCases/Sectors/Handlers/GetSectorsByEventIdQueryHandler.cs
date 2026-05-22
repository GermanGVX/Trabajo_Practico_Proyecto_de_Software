using Application.Interfaces;
using Application.UseCases.Sectors.Querys;

namespace Application.UseCases.Sectors.Handlers
{
    public class GetSectorsByEventIdQueryHandler : IGetSectorsByEventIdQueryHandler
    {
        private readonly ISectorRepository _query;

        public GetSectorsByEventIdQueryHandler(ISectorRepository query)
        {
            _query = query;
        }

        public async Task<List<GetSectorsByEventIdQuery>> GetSectorByEventId(int eventId)
        {
            var sector = await _query.GetByEventIdAsync(eventId);

            return sector.Select(s => new GetSectorsByEventIdQuery
            {
                Id = s.Id,
                Name = s.Name,
                Price = s.Price,
                Capacity = s.Capacity,
            }).ToList();
        }
    }
}
