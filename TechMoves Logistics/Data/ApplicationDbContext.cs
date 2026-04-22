using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TechMoves_Logistics.Models;

namespace TechMoves_Logistics.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Client → Contracts (One-to-Many)
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany(cl => cl.Contracts)
                .HasForeignKey(c => c.ClientId)
                // Prevent cascade delete
                .OnDelete(DeleteBehavior.Restrict);

            // Contract → ServiceRequests (One-to-Many)
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Contract)
                .WithMany(c => c.ServiceRequests)
                .HasForeignKey(sr => sr.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for search performance
            modelBuilder.Entity<Contract>()
                .HasIndex(c => c.Status);

            modelBuilder.Entity<Contract>()
                .HasIndex(c => c.StartDate);

            modelBuilder.Entity<Contract>()
                .HasIndex(c => c.EndDate);
        }
    }
}
