using Application.Interfaces;
using Application.UseCases.Seats.Querys;

namespace Application.UseCases.Seats.Handlers
{
    public class GetSeatsBySectorIdQueryHandler : IGetSeatBySectorIdQueryHandler
    {
        private readonly ISeatRepository _query;

        public GetSeatsBySectorIdQueryHandler(ISeatRepository query)
        {
            _query = query;
        }

        public async Task<List<GetSeatsBySectorIdQuery>> GetSeatBySectorId(int sectorId)
        {
            var seat = await _query.GetBySectorIdAsync(sectorId);
            return seat.Select(s => new GetSeatsBySectorIdQuery
            {
                Id = s.Id,
                SectorId = s.SectorId,
                RowIdentifier = s.RowIdentifier,
                SeatNumber = s.SeatNumber,
                Status = s.Status
            }).ToList();

        }
    }
}
