using Microsoft.EntityFrameworkCore;

namespace CafeteriaManagementSystem.CodeFirstModels
{
    public class CafeteriaCodeFirstContext : DbContext
    {
        public CafeteriaCodeFirstContext(
            DbContextOptions<CafeteriaCodeFirstContext> options
        ) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        // Configure relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(x => x.PurchaseOrder)
                .WithMany()
                .HasForeignKey(x => x.PurchaseOrderId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}