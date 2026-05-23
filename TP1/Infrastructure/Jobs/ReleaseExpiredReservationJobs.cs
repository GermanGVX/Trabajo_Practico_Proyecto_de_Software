using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Jobs
{
    public class ReleaseExpiredReservationsJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReleaseExpiredReservationsJob> _logger; 

        public ReleaseExpiredReservationsJob(IServiceProvider serviceProvider, ILogger<ReleaseExpiredReservationsJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Job de reservas expiradas iniciado.");

            // Usamos PeriodicTimer
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ProcessExpiredReservationsAsync();
            }
        }

        private async Task ProcessExpiredReservationsAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var resRepo = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
                var seatRepo = scope.ServiceProvider.GetRequiredService<ISeatRepository>();
                var auditRepo = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();

                var now = DateTime.UtcNow;
                var expired = await resRepo.GetExpiredReservationsAsync(now);

                if (!expired.Any())
                    return; 

                var seatIds = expired.Select(r => r.SeatId).Distinct().ToList();

                // Traemos todas las butacas juntas en 1 sola consulta
                var seats = await seatRepo.GetByIdsAsync(seatIds);

                var seatsToUpdate = new List<SEAT>(); 
                var reservationsToUpdate = new List<RESERVATION>();

                foreach (var reservation in expired)
                {
                    var Seat = seats.FirstOrDefault(s => s.Id == reservation.SeatId);


                    if (Seat != null && Seat.Status == nameof(SeatStatus.Reserved))
                    {
                        Seat.Status = nameof(SeatStatus.Available);
                        seatsToUpdate.Add(Seat);

                        reservation.Status = nameof(ReservationStatus.Expired);
                        reservationsToUpdate.Add(reservation);

                        await auditRepo.LogAsync(
                            action: "AUTO_RELEASE",
                            entityType: "SEAT",
                            entityId: Seat.Id.ToString(),
                            userId: null,
                            details: JsonSerializer.Serialize(new
                            {
                                SeatId = Seat.Id,
                                SeatNumber = Seat.SeatNumber,
                                Row = Seat.RowIdentifier,
                                ReservationId = reservation.Id,
                                ExpiredAt = reservation.ExpiresAt,
                                ReleasedAt = now,
                                Reason = "Tiempo de espera de pago excedido"
                            })
                        );
                    }
                }

                // Guardamos todo de golpe al final
                if (seatsToUpdate.Any())
                {
                    await seatRepo.UpdateRangeAsync(seatsToUpdate);
                    await resRepo.UpdateRangeAsync(reservationsToUpdate);

                    await seatRepo.SaveChangesAsync();

                    _logger.LogInformation($"Se liberaron {seatsToUpdate.Count} butacas expiradas.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Background Job Error]: Hubo un problema al procesar reservas expiradas.");
            }
        }
    }
}