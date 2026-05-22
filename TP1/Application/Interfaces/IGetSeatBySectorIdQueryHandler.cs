using Application.UseCases.Seats.Querys;

namespace Application.Interfaces
{
    public interface IGetSeatBySectorIdQueryHandler
    {
        Task<List<GetSeatsBySectorIdQuery>> GetSeatBySectorId(int sectorId);
    }
}
