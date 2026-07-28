
using System.Text.Json;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Reservation.Commands;
using Domain.Entities;
using Domain.Exceptions;


namespace Application.UseCases.Reservation.Handlers
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
            if (seat == null)
            {
                await _auditLogRepository.LogAsync(
                    action: "RESERVATION_FAILED_NOT_FOUND",
                    entityType: "SEAT",
                    entityId: command.SeatId.ToString(),
                    userId: command.UserId,
                    details: JsonSerializer.Serialize(new
                    {
                        RequestedSeatId = command.SeatId,
                        FailedAt = DateTime.UtcNow,
                        Reason = "Asiento no encontrado en la base de datos"
                    })
                );
                await _auditLogRepository.SaveChangesAsync(); 
                throw new KeyNotFoundException("La butaca solicitada no existe.");
            }



            if (seat.Status != "Available")
            {
                await _auditLogRepository.LogAsync(
                action: "RESERVATION_FAILED_UNAVAILABLE",
                entityType: "SEAT",
                entityId: seat.Id.ToString(),
                userId: command.UserId,
                details: JsonSerializer.Serialize(new
                {
                    SeatNumber = seat.SeatNumber,
                    Row = seat.RowIdentifier,
                    CurrentStatus = seat.Status,
                    FailedAt = DateTime.UtcNow,
                    Reason = "Butaca ya no está disponible"
                })
                );
                await _auditLogRepository.SaveChangesAsync();
                throw new InvalidOperationException("La butaca no está disponible.");

            }

            var existingReservation = await _reservationRepository.GetActiveBySeatIdAsync(command.SeatId);
            if (existingReservation != null)
            {
                await _auditLogRepository.LogAsync(
                action: "RESERVATION_CONFLICT",
                entityType: "SEAT",
                entityId: seat.Id.ToString(),
                userId: command.UserId,
                details: JsonSerializer.Serialize(new
                {
                    SeatNumber = seat.SeatNumber,
                    Row = seat.RowIdentifier,
                    ConflictAt = DateTime.UtcNow,
                    Reason = "Otro Usuario a Recervado esta butaca",
                    OptimisticLockVersion = seat.Version
                })
                );
                await _auditLogRepository.SaveChangesAsync();
                throw new InvalidOperationException("La butaca ya tiene una reserva activa.");
            }

            seat.Status = "Reserved";
            await _seatRepository.UpdateAsync(seat);

            var reservation = new RESERVATION
            {
                Id = Guid.NewGuid(),
                SeatId = command.SeatId,
                UserId = command.UserId,
                Status = "Pending",
                ReservedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            await _reservationRepository.AddAsync(reservation);

            var auditDetails = JsonSerializer.Serialize(new
            {
                SeatNumber = seat.SeatNumber,
                Row = seat.RowIdentifier,
                SectorId = seat.SectorId,
                ReservationId = reservation.Id,
                ExpiresAt = reservation.ExpiresAt,
                ReservedAt = reservation.ReservedAt,
                TimeoutMinutes = 5
            });

            await _auditLogRepository.LogAsync(
                action: "RESERVATION_SUCCESS",
                entityType: "SEAT",
                entityId: seat.Id.ToString(),
                userId: command.UserId,
                details: auditDetails
            );
            try
            {
                await _seatRepository.SaveChangesAsync();

                
            }
            catch (Exception ex) when (ex.GetType().Name == "DbUpdateConcurrencyException")
            {
                await _auditLogRepository.LogAsync(
                    action: "RESERVATION_DB_CONFLICT",
                    entityType: "SEAT",
                    entityId: command.SeatId.ToString(),
                    userId: command.UserId,
                    details: JsonSerializer.Serialize(new { ConflictAt = DateTime.UtcNow, Reason = "Optimistic Locking" })
                );
                await _auditLogRepository.SaveChangesAsync();

                throw new ConflictException("La butaca fue reservada por otro usuario.");
            }

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

    }
}
