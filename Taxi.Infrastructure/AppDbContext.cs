using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxi.Domain.Entities;
using Taxi.Domain.Enums;

namespace Taxi.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<DriverApplication> DriverApplications { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleApplication> VehicleApplications { get; set; }
        public DbSet<Ride> Rides { get; set; }
        public DbSet<Tariff> Tarrifs { get; set; }
        public DbSet<PricingSettings> PricingSettings { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<Complaints> Complaints { get; set; }
        public DbSet<GeneralInfo> GeneralInfos { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Driver>()
                .HasOne(x => x.User)
                .WithOne(x => x.Driver)
                .HasForeignKey<Driver>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Driver>()
                .HasOne(x => x.ActiveVehicle)
                .WithMany()
                .HasForeignKey(x=>x.ActiveVehicleId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<DriverApplication>()
                .HasOne(x => x.User)
                .WithMany(x => x.DriverApplications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DriverApplication>()
                .Property(r => r.Status)
                .HasConversion<string>();


            modelBuilder.Entity<Vehicle>()
                .HasOne(x => x.Driver)
                .WithMany(x => x.Vehicles)
                .HasForeignKey(x => x.DriverId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vehicle>()
                .Property(r => r.Class)
                .HasConversion<string>();


            modelBuilder.Entity<VehicleApplication>()
                .HasOne(x => x.Driver)
                .WithMany(x => x.VehicleApplications)
                .HasForeignKey(x => x.DriverId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VehicleApplication>()
                .Property(r => r.VerificationStatus)
                .HasConversion<string>();

            modelBuilder.Entity<VehicleApplication>()
                .Property(r => r.Class)
                .HasConversion<string>();


            modelBuilder.Entity<Ride>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Ride>()
                .Property(r => r.CancellationReason)
                .HasConversion<string>();

            modelBuilder.Entity<Ride>()
                .Property(r => r.RideClass)
                .HasConversion<string>();
            
            modelBuilder.Entity<Ride>()
                .Property(x => x.PricePerKm)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Ride>()
                .Property(x => x.TotalPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Ride>()
                .Property(r => r.PaymentMethod)
                .HasConversion<string>();

            modelBuilder.Entity<Ride>()
                .HasOne(x => x.Passenger)
                .WithMany(x => x.Rides)
                .HasForeignKey(x => x.PassengerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ride>()
                .HasOne(x => x.Driver)
                .WithMany(x => x.Rides)
                .HasForeignKey(x => x.DriverId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Tariff>()
                .HasIndex(x => x.Class)
                .IsUnique();

            modelBuilder.Entity<Tariff>()
                .Property(x => x.BaseFare)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Tariff>()
                .Property(x => x.WaitingPricePerMin)
                .HasPrecision(10, 2);


            modelBuilder.Entity<PricingSettings>()
                .Property(x => x.AverageFuelCons)
                .HasPrecision(10, 2);

            modelBuilder.Entity<PricingSettings>()
                .Property(x => x.AverageFuelPrice)
                .HasPrecision(10, 2);


            modelBuilder.Entity<Rating>()
                .HasOne<Ride>()
                .WithMany()
                .HasForeignKey(r => r.RideId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rating -> User (автор оцінки)
            modelBuilder.Entity<Rating>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rating -> User (отримувач оцінки)
            modelBuilder.Entity<Rating>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rating>()
                .Property(r => r.Score)
                .IsRequired();

            modelBuilder.Entity<Rating>()
                .Property(r => r.Comment)
                .HasMaxLength(1000);

            modelBuilder.Entity<Rating>()
                .Property(x => x.Tags)
                .HasConversion<string>();
            
            modelBuilder.Entity<Rating>()
                .HasOne<User>()
                .WithMany(u => u.GivenRatings)
                .HasForeignKey(r => r.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rating>()
                .HasOne<User>()
                .WithMany(u => u.ReceivedRatings)
                .HasForeignKey(r => r.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            #region Seed
            modelBuilder.Entity<GeneralInfo>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Початковий сід даних при створенні міграції
                entity.HasData(new GeneralInfo
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    TotalUsers = 0,
                    TotalRides = 0,
                    TotalComplaints = 0,
                    TotalEarnedFromComission = 0,
                });
            });

            modelBuilder.Entity<Tariff>().HasData(
                new Tariff
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    Class = VehicleClass.Econom,
                    BaseFare = 40,
                    WaitingPricePerMin = 3
                },
                new Tariff
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    Class = VehicleClass.Standart,
                    BaseFare = 50,
                    WaitingPricePerMin = 4
                },
                new Tariff
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    Class = VehicleClass.Comfort,
                    BaseFare = 70,
                    WaitingPricePerMin = 5
                },
                new Tariff
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                    Class = VehicleClass.Business,
                    BaseFare = 100,
                    WaitingPricePerMin = 8
                },
                new Tariff
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                    Class = VehicleClass.Minivan,
                    BaseFare = 90,
                    WaitingPricePerMin = 6
                }
            );

            modelBuilder.Entity<PricingSettings>().HasData(
                new Domain.Entities.PricingSettings
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    AverageFuelCons = 9,
                    AverageFuelPrice = 94.5
                }
            );
            #endregion
        }
    }
}
