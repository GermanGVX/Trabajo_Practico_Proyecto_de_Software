using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

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

            await _context.Events.AddAsync(Event);
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

        public async Task<List<EVENT>> GetListEvents()
        {
            return await _context.Events
                .AsNoTracking()
                .ToListAsync();
        }

        public EVENT GetEvent(int eventId)
        {
            var events = _context.Events
                .FirstOrDefault(e => e.Id == eventId);
            return events;
        }
    }
}
