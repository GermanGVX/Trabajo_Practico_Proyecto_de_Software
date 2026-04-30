
﻿using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Task UpdateAsync(SEAT seat)
        {
            _context.seat.Update(seat);
            return Task.CompletedTask;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
