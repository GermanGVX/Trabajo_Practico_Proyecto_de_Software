using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class SectorRepository : ISectorRepository
    {
        private readonly AppDbContext _context;

        public SectorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SECTOR>> GetByEventIdAsync(int eventId)
        {
            return await _context.sector
                .Where(s => s.EventId == eventId)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}
