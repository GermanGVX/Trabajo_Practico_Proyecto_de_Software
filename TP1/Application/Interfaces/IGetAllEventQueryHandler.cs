using Application.UseCases.Events.Querys;

namespace Application.Interfaces
{
    public interface IGetAllEventsQueryHandler
    {
        Task<List<GetAllEventsQuery>> GetAllEvents();
    }
}
