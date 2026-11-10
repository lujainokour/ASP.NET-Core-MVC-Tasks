using Microsoft.EntityFrameworkCore;
using UniversityCafeteriaManagement1.Models;

namespace UniversityCafeteriaManagement1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options
        ) : base(options)
        {
        }

        public DbSet<Category1> Categories { get; set; }
        public DbSet<Supplier1> Suppliers { get; set; }
        public DbSet<Product1> Products { get; set; }
        public DbSet<Employee1> Employees { get; set; }
        public DbSet<Order1> Orders { get; set; }
    }
}