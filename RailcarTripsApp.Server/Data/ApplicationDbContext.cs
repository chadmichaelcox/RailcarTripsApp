using Microsoft.EntityFrameworkCore;
using RailcarTripsApp.Shared.Models;

namespace RailcarTripsApp.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Trip> Trips { get; set; }
        public DbSet<EquipmentEvent> EquipmentEvents { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<EventCodeDefinition> EventCodes { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trip>()
                .Property(t => t.TotalTripHours)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<EquipmentEvent>()
                .HasOne(e => e.City)
                .WithMany()
                .HasForeignKey(e => e.CityId);

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.DestinationCity)
                .WithMany()
                .HasForeignKey(t => t.DestinationCityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.OriginCity)
                .WithMany()
                .HasForeignKey(t => t.OriginCityId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
