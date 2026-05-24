using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<USER> Users { get; set; }
        public DbSet<SECTOR> Sectors { get; set; }
        public DbSet<SEAT> Seats { get; set; }
        public DbSet<RESERVATION> Reservations { get; set; }
        public DbSet<EVENT> Events { get; set; }
        public DbSet<AUDIT_LOG> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EVENT>(entity =>
            {
                entity.ToTable("EVENT");

                entity.HasKey(e => e.Id);

                entity.Property(t => t.Id).ValueGeneratedOnAdd();

                // Límites de string
                entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Venue).HasMaxLength(150);
                entity.Property(e => e.Status).HasMaxLength(50);

                // Índice
                entity.HasIndex(e => e.Name);
            });

            modelBuilder.Entity<SECTOR>(entity =>
            {
                entity.ToTable("SECTOR");

                entity.HasKey(e => e.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();

                // El HasPrecision
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();

                entity.HasOne<EVENT>(e => e.Events)
                .WithMany(e => e.sectors)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SEAT>(entity =>
            {
                entity.ToTable("SEAT");
                entity.HasKey(e => e.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();

                // Límites
                entity.Property(e => e.RowIdentifier).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();

                entity.HasOne<SECTOR>(e => e.sector)
                .WithMany(e => e.Seats)
                .HasForeignKey(e => e.SectorId)
                .OnDelete(DeleteBehavior.Cascade);



                entity.Property(s => s.Version).IsConcurrencyToken();

                // Índice
                entity.HasIndex(e => new { e.SectorId, e.RowIdentifier, e.SeatNumber }).IsUnique();
            });

            modelBuilder.Entity<RESERVATION>(entity =>
            {
                entity.ToTable("RESERVATION");
                entity.HasKey(e => e.Id);

                // Límite para el estado
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(r => r.user)
                    .WithMany(u => u.reserva)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(r => r.seat)
                    .WithMany()
                    .HasForeignKey(r => r.SeatId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Índice
                entity.HasIndex(e => e.ExpiresAt);
            });

            modelBuilder.Entity<USER>(entity =>
            {
                entity.ToTable("USER");

                entity.HasKey(e => e.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).HasMaxLength(150);
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.PasswordHash).HasMaxLength(255);

                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<AUDIT_LOG>(entity =>
            {
                entity.ToTable("AUDIT_LOG");

                entity.HasKey(e => e.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Action).HasMaxLength(100);
                entity.Property(e => e.EntityType).HasMaxLength(100);

                entity.HasOne<USER>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            });


            SeedData(modelBuilder);
        }
        private void SeedData(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<EVENT>().HasData(
                new EVENT
                {
                    Id = 1,
                    Name = "Concierto de Rock",
                    EventDate = new DateTime(2026, 6, 15, 20, 0, 0),
                    Venue = "Estadio Nacional",
                    Status = "Activo"
                }
            );

            // SECTORES
            modelBuilder.Entity<SECTOR>().HasData(
                new SECTOR { Id = 1, EventId = 1, Name = "Campo", Price = 5000.00m, Capacity = 50 },
                new SECTOR { Id = 2, EventId = 1, Name = "Platea", Price = 8000.00m, Capacity = 50 }
            );

            // BUTACAS - GUIDs estáticos (formato 8-4-4-4-12)
            var seats = new List<SEAT>();

            // Campo: GUIDs base "10000000-0000-0000-0000-000000000XXX"
            for (int i = 1; i <= 50; i++)
            {
                string guidHex = i.ToString("X3").PadLeft(3, '0');
                string guidStr = $"10000000-0000-0000-0000-000000000{guidHex}";

                seats.Add(new SEAT
                {
                    Id = Guid.Parse(guidStr),
                    SectorId = 1,
                    RowIdentifier = "A",
                    SeatNumber = i,
                    Status = "Available",
                    Version = 0
                });
            }

            // Platea: GUIDs base "20000000-0000-0000-0000-000000000XXX"
            for (int i = 1; i <= 50; i++)
            {
                string guidHex = i.ToString("X3").PadLeft(3, '0');
                string guidStr = $"20000000-0000-0000-0000-000000000{guidHex}";

                seats.Add(new SEAT
                {
                    Id = Guid.Parse(guidStr),
                    SectorId = 2,
                    RowIdentifier = "B",
                    SeatNumber = i,
                    Status = "Available",
                    Version = 0
                });
            }

            modelBuilder.Entity<SEAT>().HasData(seats);
        }
    }
}
