using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Seats.Querys;
using Domain.Enums;

namespace Application.UseCases.Seats.Handlers
{
    public class GetSeatsBySectorIdQueryHandler : IGetSeatBySectorIdQueryHandler
    {
        private readonly ISeatRepository _query;

        public GetSeatsBySectorIdQueryHandler(ISeatRepository query)
        {
            _query = query;
        }

        public async Task<List<GroupedSeatsResponseDto>> GetSeatBySectorId(int sectorId)
        {
            var seats = await _query.GetBySectorIdAsync(sectorId);

            var groupedSeats = seats
                .GroupBy(s => s.RowIdentifier ?? "A")
                .OrderBy(g => g.Key)
                .Select(g => new GroupedSeatsResponseDto
                {
                    Row = g.Key,
                    Seats = g.Select(s => new SeatResponseDto
                    {
                        Id = s.Id,
                        SeatNumber = s.SeatNumber,
                        RowIdentifier = s.RowIdentifier ?? "A",

                        Status = Enum.Parse<SeatStatus>(s.Status, ignoreCase: true)

                    }).OrderBy(s => s.SeatNumber).ToList()
                })
                .ToList();

            return groupedSeats;
        }
    }
}
