using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace BabyStore.Models
{
    public class StoreContext:DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options):base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductImageMapping> ProductImageMappings { get; set; }
        public DbSet<BasketLine> BasketLines { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductImage>()
                .HasIndex(i=>i.FileName).HasName("FileNameIndex").IsUnique();
            modelBuilder.Entity<ProductImageMapping>()
                .HasIndex(pim=>new { pim.ProductID,pim.ProductImageID})
                .HasName("ImageIndex").IsUnique();
            base.OnModelCreating(modelBuilder); 
        }
    }
}
