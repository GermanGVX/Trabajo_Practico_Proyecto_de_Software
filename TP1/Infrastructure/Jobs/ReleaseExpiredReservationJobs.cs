using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Jobs
{
    // 1. Heredamos de BackgroundService (Moderno y nativo para tareas asíncronas)
    public class ReleaseExpiredReservationsJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReleaseExpiredReservationsJob> _logger; // 2. Agregamos el Logger

        public ReleaseExpiredReservationsJob(IServiceProvider serviceProvider, ILogger<ReleaseExpiredReservationsJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // 3. Este método reemplaza al StartAsync y al DoWork. ¡Y devuelve Task!
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Job de reservas expiradas iniciado.");

            // Usamos PeriodicTimer (no te obliga a usar async void)
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
                    return; // Si no hay nada vencido, cortamos acá para ahorrar recursos

                // --- 4. SOLUCIÓN AL PROBLEMA N+1 ---
                // En vez de buscar asiento por asiento en un foreach, agarramos todos los IDs
                var seatIds = expired.Select(r => r.SeatId).Distinct().ToList();

                // Traemos todas las butacas juntas en 1 sola consulta
                // (Nota: Vas a tener que crear este método GetByIdsAsync en tu ISeatRepository)
                var seats = await seatRepo.GetByIdsAsync(seatIds);

                var seatsToUpdate = new List<SEAT>(); // Asumiendo que tu entidad se llama Seat
                var reservationsToUpdate = new List<RESERVATION>();

                foreach (var reservation in expired)
                {
                    var Seat = seats.FirstOrDefault(s => s.Id == reservation.SeatId);

                    // 5. SOLUCIÓN A LOS STRINGS HARDCODEADOS
                    // Usamos nameof() simulando que tenés un Enum, o directamente el Enum si tu BD lo soporta
                    if (Seat != null && Seat.Status == nameof(SeatStatus.Reserved))
                    {
                        Seat.Status = nameof(SeatStatus.Available);
                        seatsToUpdate.Add(Seat);

                        reservation.Status = nameof(ReservationStatus.Expired);
                        reservationsToUpdate.Add(reservation);

                        // El audit puede quedar individual o podés hacer un LogRangeAsync si tenés muchos
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

                // Guardamos todo de golpe al final (1 solo viaje a la BD)
                if (seatsToUpdate.Any())
                {
                    await seatRepo.UpdateRangeAsync(seatsToUpdate); // Creado en tu repo para procesar listas
                    await resRepo.UpdateRangeAsync(reservationsToUpdate);

                    await seatRepo.SaveChangesAsync();

                    _logger.LogInformation($"Se liberaron {seatsToUpdate.Count} butacas expiradas.");
                }
            }
            catch (Exception ex)
            {
                // Reemplazamos el Console.WriteLine por el Logger
                _logger.LogError(ex, "[Background Job Error]: Hubo un problema al procesar reservas expiradas.");
            }
        }
    }
}