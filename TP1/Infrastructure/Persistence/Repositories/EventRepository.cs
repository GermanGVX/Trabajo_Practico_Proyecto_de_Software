using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task InsertEvent(EVENT Event)
        {

            await _context.events.AddAsync(Event);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<EVENT>> GetListEvents()
        {
            return await _context.events
                .AsNoTracking()
                .ToListAsync();
        }

        public EVENT GetEvent(int eventId)
        {
            var events = _context.events
                .FirstOrDefault(e => e.Id == eventId);
            return events; 
        }
    }
}
