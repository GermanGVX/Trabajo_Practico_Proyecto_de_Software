using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    internal class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> user { get; set; }
        public DbSet<Sector> sector { get; set; }
        public DbSet<Seat> seat { get; set; }
        public DbSet<Reservation> reservation { get; set;}
        public DbSet<Event> events { get; set; }
        public DbSet<Audit_Log> audit { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(t => t.Id).ValueGeneratedOnAdd();
                
            });

            modelBuilder.Entity<Sector>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(t  => t.Id).ValueGeneratedOnAdd();

                entity.HasOne<Event>(e => e.Events)
                .WithMany(e => e.sectors)
                .HasForeignKey(e => e.EventId);
            });

            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(t => t.id).ValueGeneratedOnAdd();

                entity.HasOne<Sector>(e => e.sector)
                .WithMany(e => e.Seats)
                .HasForeignKey(e => e.SectorId);
            });

            
                
        }
    }
}
