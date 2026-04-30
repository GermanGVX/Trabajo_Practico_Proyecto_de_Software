using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
