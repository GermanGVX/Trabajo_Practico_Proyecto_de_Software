using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(USER user);
        Task SaveChangesAsync();
        USER GetUser(int id);
        Task<USER?> GetByEmailAsync(string email);
    }
}
