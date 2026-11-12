using Microsoft.EntityFrameworkCore;
using ECommerce.API.Models;

namespace ECommerce.API.Data
{
    public class ECommerceDbContext : DbContext
    {
        public ECommerceDbContext(
            DbContextOptions<ECommerceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }
    }
}