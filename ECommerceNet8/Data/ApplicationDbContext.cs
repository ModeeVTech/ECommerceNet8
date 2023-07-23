using ECommerceNet8.Configurations;
using ECommerceNet8.Models.AuthModels;
using ECommerceNet8.Models.OrderModels;
using ECommerceNet8.Models.ProductModels;
using ECommerceNet8.Models.ShoppingCartModels;
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

            builder.Entity<Order>()
                .HasOne(o => o.OrignalOrderFromCustomer)
                .WithOne(oo => oo.Order)
                .HasForeignKey<OrderFromCustomer>(oc => oc.OrderId)
                .IsRequired(false);

            builder.Entity<OrderFromCustomer>()
                .HasOne(oc=>oc.pdfInfo)
                .WithOne(pi=>pi.OrderFromCustomer)
                .HasForeignKey<PdfInfo>(pi=>pi.OrderFromCustomerId)
                .IsRequired(false);
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<MainCategory> MainCategories { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<BaseProduct> BaseProducts { get; set; }
        public DbSet<ImageBase> ImageBases { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderFromCustomer> OrdersFromCustomers { get; set; }
        public DbSet<OrderItem> OrdersItems { get; set;}
        public DbSet<PdfInfo> PdfInfos { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ShippingType> ShippingTypes { get; set; }
        public DbSet<ReturnedItemsFromCustomer> returnedItemsFromCustomers { get; set; }
    }
    
}
