using Application.UseCases.Events.Commands;

namespace Application.Interfaces
{
    public interface ICreateEventCommandHandler
    {
        Task<int> CreateEvent(CreateEventCommand command);
    }
}
