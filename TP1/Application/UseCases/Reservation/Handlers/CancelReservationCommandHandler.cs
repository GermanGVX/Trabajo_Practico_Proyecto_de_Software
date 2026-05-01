using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.UseCases.Reservation.Commands;

namespace Application.UseCases.Reservation.Handlers
{
    public class CancelReservationCommandHandler : ICancelReservationCommandHandler
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly ISeatRepository _seatRepo;
        private readonly IAuditLogRepository _auditRepo;

        public CancelReservationCommandHandler(
            IReservationRepository reservationRepo,
            ISeatRepository seatRepo,
            IAuditLogRepository auditRepo)
        {
            _reservationRepo = reservationRepo;
            _seatRepo = seatRepo;
            _auditRepo = auditRepo;
        }

        public async Task CancelReservation(CancelReservationCommand command)
        {
            // 1. Buscar reserva
            var reservation = await _reservationRepo.GetByIdAsync(command.ReservationId);
            if (reservation == null)
                throw new KeyNotFoundException("Reserva no encontrada.");

            if (reservation.Status != "Pending")
                throw new InvalidOperationException("La reserva ya no está pendiente.");

            // 2. Liberar butaca si está reservada
            var seat = await _seatRepo.GetByIdAsync(reservation.SeatId);
            if (seat != null && seat.Status == "Reserved")
            {
                seat.Status = "Available";
                await _seatRepo.UpdateAsync(seat);
            }

            // 3. Marcar reserva como cancelada
            reservation.Status = "Expired";
            await _reservationRepo.UpdateAsync(reservation);

            // 4. Commit
            await _reservationRepo.SaveChangesAsync();

            // 5. Auditoría 
            await _auditRepo.LogAsync(
                action: "RESERVATION_CANCELLED",
                entityType: "RESERVATION",
                entityId: reservation.Id.ToString(),
                userId: reservation.UserId,
                details: JsonSerializer.Serialize(new
                {
                    ReservationId = reservation.Id,
                    SeatId = reservation.SeatId,
                    CancelledAt = DateTime.UtcNow,
                    TimeRemaining = TimeSpan.FromSeconds((reservation.ExpiresAt - DateTime.UtcNow).TotalSeconds),
                    Reason = "Cancelación manual del usuario"
                })
            );
        }
    }
}
