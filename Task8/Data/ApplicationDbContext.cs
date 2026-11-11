using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniversityCafeteriaSupply8.Models;

namespace UniversityCafeteriaSupply8.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<SupplyRequest> SupplyRequests { get; set; }

        public DbSet<RequestItem> RequestItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SupplyRequest>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RequestItem>()
                .HasOne<SupplyRequest>()
                .WithMany()
                .HasForeignKey(i => i.SupplyRequestId);

            builder.Entity<RequestItem>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Entity<SupplyRequest>()
                .Property(r => r.TotalPrice)
                .HasPrecision(18, 2);

            builder.Entity<RequestItem>()
                .Property(i => i.Price)
                .HasPrecision(18, 2);
        }
    }
}