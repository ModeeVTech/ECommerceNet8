using ECommerceNet8.Configurations;
using ECommerceNet8.Models.AuthModels;
using ECommerceNet8.Models.ProductModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerceNet8.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApiUser>
    {
        public ApplicationDbContext(DbContextOptions options)  : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new RoleConfiguration());
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<MainCategory> MainCategories { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<BaseProduct> BaseProducts { get; set; }
        public DbSet<ImageBase> ImageBases { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
    }
    
}
