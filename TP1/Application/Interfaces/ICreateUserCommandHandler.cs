using Application.UseCases.Events.Commands;

namespace Application.Interfaces
{
    public interface ICreateUserCommandHandler
    {
        Task<int> CreateUser(CreateUserCommand command);
    }
}
