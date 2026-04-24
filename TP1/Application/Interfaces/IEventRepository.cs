using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interface
{
    public interface IEventRepository
    {
        Task InsertEvent(EVENT Event);
        Task SaveChangesAsync();
        Task<List<EVENT>> GetListEvents();
        EVENT GetEvent(int eventId);
    }
}
