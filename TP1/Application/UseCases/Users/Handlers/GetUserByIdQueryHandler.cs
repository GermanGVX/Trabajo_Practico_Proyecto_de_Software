using Application.Interfaces;
using Application.UseCases.Events.Querys;

namespace Application.UseCases.Users.Handlers
{
    public class GetUserByIdQueryHandler : IGetUserByIdQueryHandler
    {
        private readonly IUserRepository _query;

        public GetUserByIdQueryHandler(IUserRepository query)
        {
            _query = query;
        }

        public Task<GetUserByIdQuery> GetUserById(int id)
        {
            var user = _query.GetUser(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"Usuario Con ID {id} No Encontrado");
            }
            return Task.FromResult(new GetUserByIdQuery
            {
                Name = user.Name,
                Email = user.Email
            });

        }
    }
}
