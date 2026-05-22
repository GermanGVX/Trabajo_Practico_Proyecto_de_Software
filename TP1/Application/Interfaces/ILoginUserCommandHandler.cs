using Application.UseCases.Users.Commands;

namespace Application.Interfaces
{
    public interface ILoginUserCommandHandler
    {
        Task<int> LoginUser(LoginUserCommand command);
    }
}
