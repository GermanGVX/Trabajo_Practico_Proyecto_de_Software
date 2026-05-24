
using Domain.Entities;


namespace Application.Interfaces
{
    public interface ISeatRepository
    {

        Task<List<SEAT>> GetBySectorIdAsync(int sectorId);
        Task<SEAT?> GetByIdAsync(Guid id);

        Task<List<SEAT>> GetByIdsAsync(IEnumerable<Guid> ids);
        Task UpdateRangeAsync(IEnumerable<SEAT> seats);

        Task UpdateAsync(SEAT seat);
        Task SaveChangesAsync();

    }
}
