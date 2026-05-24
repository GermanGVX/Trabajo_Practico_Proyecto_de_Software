using Domain.Entities;

namespace Application.Interfaces
{
    public interface IEventRepository
    {
        Task InsertEvent(EVENT Event);
        Task SaveChangesAsync();
        Task<List<EVENT>> GetListEvents();
        EVENT GetEvent(int eventId);
    }
}
