using Microsoft.EntityFrameworkCore;

namespace Platform.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> ops) : base(ops) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
    }
}
