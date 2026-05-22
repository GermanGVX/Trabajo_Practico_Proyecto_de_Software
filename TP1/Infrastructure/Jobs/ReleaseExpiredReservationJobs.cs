using System.Text.Json;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Jobs
{
    public class ReleaseExpiredReservationsJob : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;

        public ReleaseExpiredReservationsJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {

                using var scope = _serviceProvider.CreateScope();
                var resRepo = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
                var seatRepo = scope.ServiceProvider.GetRequiredService<ISeatRepository>();
                var auditRepo = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();

                var now = DateTime.UtcNow;
                var expired = await resRepo.GetExpiredReservationsAsync(now);

                foreach (var reservation in expired)
                {
                    var seat = await seatRepo.GetByIdAsync(reservation.SeatId);
                    if (seat?.Status == "Reserved")
                    {

                        seat.Status = "Available";
                        await seatRepo.UpdateAsync(seat);


                        reservation.Status = "Expired";
                        await resRepo.UpdateAsync(reservation);


                        await auditRepo.LogAsync(
                            action: "AUTO_RELEASE",
                            entityType: "SEAT",
                            entityId: seat.Id.ToString(),
                            userId: null,
                            details: JsonSerializer.Serialize(new
                            {
                                SeatId = seat.Id,
                                SeatNumber = seat.SeatNumber,
                                Row = seat.RowIdentifier,
                                ReservationId = reservation.Id,
                                ExpiredAt = reservation.ExpiresAt,
                                ReleasedAt = DateTime.UtcNow,
                                OverdueBy = TimeSpan.FromSeconds((DateTime.UtcNow - reservation.ExpiresAt).TotalSeconds),
                                Reason = "Tiempo de espera de pago: se excedieron 5 minutos sin confirmación"
                            })
                        );
                    }
                }

                if (expired.Count > 0)
                {
                    await seatRepo.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"[Background Job Error]: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
