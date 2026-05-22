using Application.UseCases.Events.Querys;

namespace Application.Interfaces
{
    public interface IGetEventByIdQueryHandler
    {
        public Task<GetEventByIdQuery> GetEventById(int eventId);
    }
}
