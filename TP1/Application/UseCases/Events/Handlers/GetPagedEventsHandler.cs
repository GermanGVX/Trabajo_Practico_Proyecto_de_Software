using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Events.Querys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Events.Handlers
{
    public class GetPagedEventsHandler : IGetPagedEventsHandler
    {
        private readonly IEventRepository _repository;

        public GetPagedEventsHandler(IEventRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponseDto<EventResponseDto>> GetPagedEvents(GetPagedEventsQuery request)
        {
            var (events, total) = await _repository.GetPagedAsync(request.Page, request.PageSize);

            // ¡Acá agregamos el mapeo de los datos!
            var dtoList = events.Select(e => new EventResponseDto
            {
                Id = e.Id,
                Name = e.Name,
                EventDate = e.EventDate,
                Venue = e.Venue
            }).ToList();

            return new PagedResponseDto<EventResponseDto>
            {
                Data = dtoList,
                Total = total,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
