using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Data
{
    public class ApplicationDBContext : DbContext
    {

        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Shirt> Shirts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeding initial data
            modelBuilder.Entity<Shirt>().HasData(
                new Shirt { ShirtId = 1, Brand = "Nike", Color = "Red", Size = 10, Gender = "mens", Price = 100 },
                new Shirt { ShirtId = 2, Brand = "Adidas", Color = "Blue", Size = 12, Gender = "womens", Price = 155.5 },
                new Shirt { ShirtId = 3, Brand = "Puma", Color = "Green", Size = 9, Gender = "mens", Price = 200 }
                );

        }
    }
}
