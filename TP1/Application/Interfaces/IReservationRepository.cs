using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IReservationRepository
    {
        Task AddAsync(RESERVATION reservation);
        Task<RESERVATION?> GetActiveBySeatIdAsync(Guid seatId);
        Task<List<RESERVATION>> GetExpiredReservationsAsync(DateTime threshold);
        Task UpdateAsync(RESERVATION reservation);
        Task SaveChangesAsync();
        
    }
}
