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
            return await _context.seat.FindAsync(id);
        }

        public async Task<List<SEAT>> GetBySectorIdAsync(int sectorId)
        {
            return await _context.seat
                .Where(s => s.SectorId == sectorId)
                .OrderBy(s => s.SeatNumber)
                .ToListAsync();
        }

        // --- NUEVO MÉTODO PARA SOLUCIONAR N+1 ---
        public async Task<List<SEAT>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.seat
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();
        }

        public Task UpdateAsync(SEAT seat)
        {
            _context.seat.Update(seat);
            return Task.CompletedTask;
        }

        // --- NUEVO MÉTODO PARA ACTUALIZACIÓN MASIVA (N+1) ---
        public Task UpdateRangeAsync(IEnumerable<SEAT> seats)
        {
            _context.seat.UpdateRange(seats);
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