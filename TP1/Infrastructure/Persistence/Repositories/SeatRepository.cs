<<<<<<< HEAD
﻿using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
=======
﻿using System;
>>>>>>> f326cc82e92634ce93fd468514a8591c3af94a97
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD
=======
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
>>>>>>> f326cc82e92634ce93fd468514a8591c3af94a97

namespace Infrastructure.Persistence.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly AppDbContext _context;

        public SeatRepository(AppDbContext context)
        {
            _context = context;
<<<<<<< HEAD

        }
=======
        }
        public async Task<SEAT?> GetByIdAsync(Guid id)
        {
            return await _context.seat.FindAsync(id);
        }

>>>>>>> f326cc82e92634ce93fd468514a8591c3af94a97
        public async Task<List<SEAT>> GetBySectorIdAsync(int sectorId)
        {
            return await _context.seat
                .Where(s => s.SectorId == sectorId)
<<<<<<< HEAD
                .OrderBy(s => s.RowIdentifier)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();
        }
=======
                .OrderBy(s => s.SeatNumber)
                .ToListAsync();
        }
        public Task UpdateAsync(SEAT seat)
        {
            _context.seat.Update(seat);
            return Task.CompletedTask;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
>>>>>>> f326cc82e92634ce93fd468514a8591c3af94a97
    }
}
