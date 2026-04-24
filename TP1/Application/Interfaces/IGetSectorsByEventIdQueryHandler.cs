using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UseCases.Sectors.Querys;

namespace Application.Interfaces
{
    public interface IGetSectorsByEventIdQueryHandler
    {
        Task<List<GetSectorsByEventIdQuery>> GetSectorByEventId(int eventId);
    }
}
