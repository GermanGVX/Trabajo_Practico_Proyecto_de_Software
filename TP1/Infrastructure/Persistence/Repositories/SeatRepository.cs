using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly AppDbContext _context;

        public SeatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SEAT?> GetByIdAsync(Guid id)
        {
            return await _context.Seats.FindAsync(id);
        }

        public async Task<List<SEAT>> GetBySectorIdAsync(int sectorId)
        {
            return await _context.Seats
                .Where(s => s.SectorId == sectorId)
                .OrderBy(s => s.SeatNumber)
                .ToListAsync();
        }

        public async Task<List<SEAT>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.Seats
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();
        }

        public Task UpdateAsync(SEAT seat)
        {
            _context.Seats.Update(seat);
            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(IEnumerable<SEAT> seats)
        {
            _context.Seats.UpdateRange(seats);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException("Conflicto de concurrencia: el recurso fue modificado por otro usuario.");
            }
        }
    }
}