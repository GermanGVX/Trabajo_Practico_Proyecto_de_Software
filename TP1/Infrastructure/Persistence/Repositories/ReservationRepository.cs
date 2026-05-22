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
            await _context.reservation.AddAsync(reservation);
        }

        public async Task<RESERVATION?> GetActiveBySeatIdAsync(Guid seatId)
        {
            return await _context.reservation
                .FirstOrDefaultAsync(r => r.SeatId == seatId && r.Status == "Pending");
        }

        public async Task<List<RESERVATION>> GetExpiredReservationsAsync(DateTime threshold)
        {
            return await _context.reservation
                .Where(r => r.Status == "Pending" && r.ExpiresAt < threshold)
                .ToListAsync();
        }

        public async Task UpdateAsync(RESERVATION reservation)
        {
            _context.reservation.Update(reservation);
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

        public async Task<RESERVATION?> GetByIdAsync(Guid id)
        {
            return await _context.reservation.FindAsync(id);
        }


    }
}
