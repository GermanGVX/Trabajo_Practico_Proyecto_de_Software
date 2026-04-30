using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
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
            await _context.SaveChangesAsync();
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
