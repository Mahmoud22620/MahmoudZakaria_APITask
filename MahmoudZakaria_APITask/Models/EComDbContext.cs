using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MahmoudZakaria_APITask.Models
{
    public class EComDbContext : IdentityDbContext<User>
    {
        public EComDbContext(DbContextOptions<EComDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductCode)
                .IsUnique();

            // Seed roles
            modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "1a111111-1111-1111-1111-111111111111",   
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "1b111111-1111-1111-1111-111111111111" 
            },
            new IdentityRole
            {
                Id = "2a222222-2222-2222-2222-222222222222", 
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = "2b222222-2222-2222-2222-222222222222"
            });


            // Seed 5 products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Category = "Electronics",
                    ProductCode = "P01",
                    Name = "Smartphone",
                    ImagePath = "images/Product.jpg",
                    Price = 699.99m,
                    MinimumQuantity = 1,
                    DiscountRate = 0.10
                },
                new Product
                {
                    Id = 2,
                    Category = "Electronics",
                    ProductCode = "P02",
                    Name = "Laptop",
                    ImagePath = "images/Product.jpg",
                    Price = 1299.99m,
                    MinimumQuantity = 1,
                    DiscountRate = 0.15
                },
                new Product
                {
                    Id = 3,
                    Category = "Home Appliances",
                    ProductCode = "P03",
                    Name = "Microwave",
                    ImagePath = "images/Product.jpg",
                    Price = 199.99m,
                    MinimumQuantity = 1,
                    DiscountRate = 0.05
                },
                new Product
                {
                    Id = 4,
                    Category = "Home Appliances",
                    ProductCode = "P04",
                    Name = "Refrigerator",
                    ImagePath = "images/Product.jpg",
                    Price = 899.99m,
                    MinimumQuantity = 1,
                    DiscountRate = 0.12
                },
                new Product
                {
                    Id = 5,
                    Category = "Accessories",
                    ProductCode = "P05",
                    Name = "Headphones",
                    ImagePath = "images/Product.jpg",
                    Price = 149.99m,
                    MinimumQuantity = 1,
                    DiscountRate = 0.08
                }
            );
        }
    }
}
