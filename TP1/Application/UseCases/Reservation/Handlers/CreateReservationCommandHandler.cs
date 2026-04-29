using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.UseCases.Events.Commands;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Application.DTOs;

namespace Application.UseCases.Events.Handlers
{
    public class CreateReservationCommandHandler : ICreateReservationCommandHandler
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public CreateReservationCommandHandler(ISeatRepository seatRepository, IReservationRepository reservationRepository, IAuditLogRepository auditLogRepository)
        {
            _seatRepository = seatRepository;
            _reservationRepository = reservationRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<ReservationResponseDto> CreateReservation(CreateReservationCommand command)
        {
            var seat = await _seatRepository.GetByIdAsync(command.SeatId);
            if (seat.Status != "Available")
                throw new InvalidOperationException("La butaca no está disponible.");

            seat.Status = "Reserved";

            var now = DateTime.UtcNow;
            var reservation = new RESERVATION
            {
                Id = Guid.NewGuid(),
                SeatId = command.SeatId,
                UserId = command.UserId,
                Status = "Pending",
                ReservedAt = now,
                ExpiresAt = now.AddMinutes(5) 
            };

            await _reservationRepository.AddAsync(reservation);
            await _seatRepository.UpdateAsync(seat);

            try
            {
                await _seatRepository.SaveChangesAsync(); 

                await _auditLogRepository.LogAsync(
                    action: "RESERVATION_SUCCESS",
                    entityType: "SEAT",
                    entityId: seat.Id.ToString(),
                    userId: command.UserId,
                    details: $"Reserva creada. Expira: {reservation.ExpiresAt:yyyy-MM-dd HH:mm:ss}"
                );

                return new ReservationResponseDto
                {
                    Id = reservation.Id,
                    SeatId = reservation.SeatId,
                    Status = reservation.Status,
                    ReservedAt = reservation.ReservedAt,
                    ExpiresAt = reservation.ExpiresAt,
                    Message = "Butaca reservada exitosamente. Tienes 5 minutos para completar el pago."
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                await _auditLogRepository.LogAsync(
                    action: "RESERVATION_CONFLICT",
                    entityType: "SEAT",
                    entityId: command.SeatId.ToString(),
                    userId: command.UserId,
                    details: "Conflicto de concurrencia"
                );

                throw new ConflictException("La butaca fue reservada por otro usuario.");
            }


        }
        
    }
}
