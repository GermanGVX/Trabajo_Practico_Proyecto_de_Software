using Application.UseCases.Events.Querys;

namespace Application.Interfaces
{
    public interface IGetUserByIdQueryHandler
    {
        Task<GetUserByIdQuery> GetUserById(int id);
    }
}
