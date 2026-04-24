using Application.Interfaces;
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
        public async Task<List<SEAT>> GetBySectorIdAsync(int sectorId)
        {
            return await _context.seat
                .Where(s => s.SectorId == sectorId)
                .OrderBy(s => s.RowIdentifier)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();
        }
    }
}
