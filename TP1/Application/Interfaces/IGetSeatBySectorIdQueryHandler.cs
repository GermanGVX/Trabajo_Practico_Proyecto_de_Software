using Application.UseCases.Seats.Querys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGetSeatBySectorIdQueryHandler
    {
        Task<List<GetSeatsBySectorIdQuery>> GetSeatBySectorId(int sectorId);
    }
}
