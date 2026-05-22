using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(USER user)
        {
            await _context.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {

                throw new ConcurrencyException(
                    "Conflicto de concurrencia: el recurso fue modificado por otro usuario."
                );
            }
        }

        public USER GetUser(int id)
        {
            var user = _context.user
                .FirstOrDefault(e => e.Id == id);
            return user;
        }
        public async Task<USER?> GetByEmailAsync(string email) =>
            await _context.user.FirstOrDefaultAsync(u => u.Email == email.ToLower());
    }
}
