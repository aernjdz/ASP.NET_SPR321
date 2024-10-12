using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DataAcess.Data.Entities;
using DataAcess.Data.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAcess.Data
{
    public class HulkDbContext : IdentityDbContext<UserEntity, RoleEntity, int>
    {
        public HulkDbContext(DbContextOptions<HulkDbContext> options) : base(options) { }

        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<CategoryEntity>(entity =>
            {
                entity.ToTable("tbl_categories"); 
                entity.HasKey(e => e.Id); 

                entity.Property(e => e.Name)
                    .IsRequired() 
                    .HasMaxLength(200); 

                entity.Property(e => e.Image)
                    .HasMaxLength(200) 
                    .HasDefaultValue(string.Empty); 
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id); 

                entity.Property(e => e.Name)
                    .IsRequired() 
                    .HasMaxLength(200); 

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)"); 

                entity.HasOne(e => e.Category) 
                    .WithMany() 
                    .HasForeignKey(e => e.CategoryId) 
                    .OnDelete(DeleteBehavior.Cascade); 

                entity.HasMany(e => e.ProductImages) 
                    .WithOne(p => p.Product) 
                    .HasForeignKey(p => p.ProductId) 
                    .OnDelete(DeleteBehavior.Cascade); 
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(500); 
                entity.Property(e => e.Priotity)
                    .IsRequired();

                entity.HasOne(e => e.Product) 
                    .WithMany(p => p.ProductImages) 
                    .HasForeignKey(e => e.ProductId) 
                    .OnDelete(DeleteBehavior.Cascade);
            });

       


          
            // product
            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId);

            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId);


            // identity 
            modelBuilder.Entity<UserRoleEntity>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRoleEntity>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.Roles)
                .HasForeignKey(ur => ur.RoleId);

        }
    }
}
