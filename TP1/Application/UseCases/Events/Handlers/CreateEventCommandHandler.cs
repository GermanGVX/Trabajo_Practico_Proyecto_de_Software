using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Application.Interfaces;
using Application.UseCases.Events.Commands;
using Domain.Entities;

namespace Application.UseCases.Events.Handlers
{
    public class CreateEventCommandHandler : ICreateEventCommandHandler
    {
        private IEventRepository _eventRepository;
        private IAuditLogRepository _auditLogRepository;

        public CreateEventCommandHandler(IEventRepository eventRepository, IAuditLogRepository auditLogRepository)
        {
            _eventRepository = eventRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<int> CreateEvent(CreateEventCommand command)
        {
            var newEvent = new EVENT
            {
                Name = command.Name.Trim(),
                EventDate = command.EventDate,
                Venue = command.Venue.Trim(),
                Status = "Activo"
            };
            await _eventRepository.InsertEvent(newEvent);

            await _auditLogRepository.LogAsync(
                action: "Create_Event",
                entityType: "Event",
                entityId: newEvent.Id.ToString(),
                userId : null,
                details : $"Evento Creado: {newEvent.Name} | Recinto: {newEvent.Venue}"
                );

            await _eventRepository.SaveChangesAsync();

            return newEvent.Id;
        }
    }
}
