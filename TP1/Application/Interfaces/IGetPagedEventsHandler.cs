using Application.DTOs;
using Application.UseCases.Events.Querys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGetPagedEventsHandler
    {
        Task<PagedResponseDto<EventResponseDto>> GetPagedEvents(GetPagedEventsQuery request);
    }
}
