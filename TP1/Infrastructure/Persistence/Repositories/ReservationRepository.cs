using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RESERVATION reservation)
        {
            await _context.Reservations.AddAsync(reservation);
        }

        public async Task<RESERVATION?> GetActiveBySeatIdAsync(Guid seatId)
        {
            return await _context.Reservations
                .FirstOrDefaultAsync(r => r.SeatId == seatId && r.Status == "Pending");
        }

        public async Task<List<RESERVATION>> GetExpiredReservationsAsync(DateTime threshold)
        {
            return await _context.Reservations
                .Where(r => r.Status == "Pending" && r.ExpiresAt < threshold)
                .ToListAsync();
        }

        public async Task<RESERVATION?> GetByIdAsync(Guid id)
        {
            return await _context.Reservations.FindAsync(id);
        }

        public Task UpdateAsync(RESERVATION reservation)
        {
            _context.Reservations.Update(reservation);
            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(IEnumerable<RESERVATION> reservations)
        {
            _context.Reservations.UpdateRange(reservations);
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
                throw new ConcurrencyException(
                    "Conflicto de concurrencia: el recurso fue modificado por otro usuario."
                );
            }
        }
    }
}