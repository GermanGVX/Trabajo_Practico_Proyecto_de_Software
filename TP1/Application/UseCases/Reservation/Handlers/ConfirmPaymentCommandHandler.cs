using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.UseCases.Reservation.Commands;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Reservation.Handlers
{
    public class ConfirmPaymentCommandHandler : IConfirmPaymentCommandHandler
    {
        private readonly IReservationRepository _reservationRepo;
        private readonly ISeatRepository _seatRepo;
        private readonly IAuditLogRepository _auditRepo;

        public ConfirmPaymentCommandHandler(IReservationRepository resRepo, ISeatRepository seatRepo, IAuditLogRepository auditRepo)
        {
            _reservationRepo = resRepo;
            _seatRepo = seatRepo;
            _auditRepo = auditRepo;
        }

        public async Task ConfirmPayment(ConfirmPaymentCommand command)
        {
            var reservation = await _reservationRepo.GetByIdAsync(command.ReservationId);
            if (reservation == null) throw new KeyNotFoundException("Reserva no encontrada.");
            if (reservation.Status != "Pending") throw new InvalidOperationException("La reserva ya fue procesada.");
            if (reservation.ExpiresAt < DateTime.UtcNow) throw new InvalidOperationException("La reserva ha expirado.");

            var seat = await _seatRepo.GetByIdAsync(reservation.SeatId);
            if (seat == null || seat.Status != "Reserved")
                throw new InvalidOperationException("Estado inconsistente de la butaca.");

            // 1. Cambiar estados
            seat.Status = "Sold";
            reservation.Status = "Confirmed";

            await _seatRepo.UpdateAsync(seat);
            await _reservationRepo.UpdateAsync(reservation);

            try
            {
                // 2. Commit atómico (ACID)
                await _seatRepo.SaveChangesAsync();

                // 3. Auditoría inmutable
                await _auditRepo.LogAsync(
                    action: "PAYMENT_CONFIRMED",
                    entityType: "RESERVATION",
                    entityId: reservation.Id.ToString(),
                    userId: reservation.UserId,
                    details: JsonSerializer.Serialize(new
                    {
                        ReservationId = reservation.Id,
                        SeatId = reservation.SeatId,
                        SeatNumber = seat.SeatNumber,
                        Row = seat.RowIdentifier,
                        ConfirmedAt = DateTime.UtcNow,
                        ReservedAt = reservation.ReservedAt,
                        TimeToComplete = TimeSpan.FromSeconds((DateTime.UtcNow - reservation.ReservedAt).TotalSeconds),
                        PaymentMethod = "Simulado"
                    })
                );
            }
            catch (DbUpdateConcurrencyException)
            {
                await _auditRepo.LogAsync(
                    action: "PAYMENT_CONFLICT",
                    entityType: "RESERVATION",
                    entityId: reservation.Id.ToString(),
                    userId: reservation.UserId,
                    details: JsonSerializer.Serialize(new
                    {
                        ReservationId = reservation.Id,
                        SeatId = reservation.SeatId,
                        ConflictAt = DateTime.UtcNow,
                        Reason = "Conflicto de concurrencia durante la confirmación del pago",
                        OptimisticLockVersion = seat.Version
                    })
                );
                throw new ConflictException("No se pudo confirmar el pago. Intente nuevamente.");
            }
        }
    }
}
