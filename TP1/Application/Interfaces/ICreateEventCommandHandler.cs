using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UseCases.Events.Commands;

namespace Application.Interfaces
{
    public interface ICreateEventCommandHandler
    {
        Task<int> CreateEvent(CreateEventCommand command);
    }
}
