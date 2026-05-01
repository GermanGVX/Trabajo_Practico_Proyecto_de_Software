using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public DbSet<USER> user { get; set; }
        public DbSet<SECTOR> sector { get; set; }
        public DbSet<SEAT> seat { get; set; }
        public DbSet<RESERVATION> reservation { get; set;}
        public DbSet<EVENT> events { get; set; }
        public DbSet<AUDIT_LOG> audit { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EVENT>(entity =>
            {
                entity.ToTable("EVENT");

                entity.HasKey(e => e.Id);

                entity.Property(t => t.Id).ValueGeneratedOnAdd();
                
            });

            modelBuilder.Entity<SECTOR>(entity =>
            {
                entity.ToTable("SECTOR");

                entity.HasKey(e => e.Id);
                entity.Property(t  => t.Id).ValueGeneratedOnAdd();

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

                entity.HasOne<SECTOR>(e => e.sector)
                .WithMany(e => e.Seats)
                .HasForeignKey(e => e.SectorId)
                .OnDelete(DeleteBehavior.Cascade);

                

                entity.Property(s => s.Version).IsConcurrencyToken();
            });

            modelBuilder.Entity<RESERVATION>(entity =>
            {
                entity.ToTable("RESERVATION");

                entity.HasKey(e => e.Id);

                entity.HasOne(r => r.user)
                    .WithMany(u => u.reserva)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(r => r.seat)
                    .WithMany()
                    .HasForeignKey(r => r.SeatId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<USER>(entity =>
            {
                entity.ToTable("USER");

                entity.HasKey(e => e.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd(); 
            });

            modelBuilder.Entity<AUDIT_LOG>(entity =>
            {
                entity.ToTable("AUDIT_LOG");

                entity.HasKey(e => e.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();

                entity.HasOne<USER>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            });


            SeedData(modelBuilder);
        }
        private void SeedData(ModelBuilder modelBuilder)
        {
            // EVENTO - Fecha fija, NO DateTime.UtcNow
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
                string guidHex = i.ToString("X3").PadLeft(3, '0'); // 001, 002, ..., 032, ..., 050
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
