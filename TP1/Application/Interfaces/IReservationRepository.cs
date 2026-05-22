using Domain.Entities;

namespace Application.Interfaces
{
    public interface IReservationRepository
    {
        Task AddAsync(RESERVATION reservation);
        Task<RESERVATION?> GetActiveBySeatIdAsync(Guid seatId);
        Task<List<RESERVATION>> GetExpiredReservationsAsync(DateTime threshold);
        Task UpdateAsync(RESERVATION reservation);
        Task SaveChangesAsync();
        Task<RESERVATION?> GetByIdAsync(Guid id);


    }
}
