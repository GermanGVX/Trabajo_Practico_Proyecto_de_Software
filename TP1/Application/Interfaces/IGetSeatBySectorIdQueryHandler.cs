using Application.DTOs;
using Application.UseCases.Seats.Querys;

namespace Application.Interfaces
{
    public interface IGetSeatBySectorIdQueryHandler
    {
        Task<List<GroupedSeatsResponseDto>> GetSeatBySectorId(int sectorId);
    }
}
