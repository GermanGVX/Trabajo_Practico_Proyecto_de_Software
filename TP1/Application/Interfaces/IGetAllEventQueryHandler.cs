using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UseCases.Events.Querys;

namespace Application.Interfaces
{
    public interface IGetAllEventsQueryHandler
    {
        Task<List<GetAllEventsQuery>> GetAllEvents();
    }
}
