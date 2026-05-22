using Domain.Entities;

namespace Application.Interfaces
{
    public interface ISectorRepository
    {
        Task<List<SECTOR>> GetByEventIdAsync(int eventId);

    }
}
