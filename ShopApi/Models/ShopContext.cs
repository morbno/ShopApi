using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql;
using ShopApi.Models.Entities;

namespace ShopApi.Models
{
    public class ShopContext : DbContext
    {
        public DbSet<ShopItemModel> ShopItems { get; set; } = null!;
        public DbSet<OrderModel> Orders { get; set; } = null!;
        public DbSet<UserModel> Users { get; set; } = null!;
        public DbSet<ShopCartModel> ShopCartRecords { get; set; } = null!;
        public DbSet<OrderItemModel> OrderRecords { get; set; } = null!;
        public DbSet<AuthTokenModel> Tokens { get; set; } = null!;

        public ShopContext(DbContextOptions<ShopContext> options) : base(options)
        {

        }
        static ShopContext() => NpgsqlConnection.GlobalTypeMapper.MapEnum<OrderStatus>("e_order_status");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new EntityConfiguration<ShopItemModel>("shop_items"));
            builder.ApplyConfiguration(new EntityConfiguration<ShopCartModel>("shop_carts"));
            builder.ApplyConfiguration(new EntityConfiguration<OrderModel>("orders"));
            builder.ApplyConfiguration(new EntityConfiguration<OrderItemModel>("order_items"));
            builder.ApplyConfiguration(new EntityConfiguration<UserModel>("users"));
            builder.ApplyConfiguration(new EntityConfiguration<AuthTokenModel>("auth_tokens"));
        }
        
        public class EntityConfiguration<T> : IEntityTypeConfiguration<T> where T : EntityModel
        {
            private readonly string _tableName;
            
            public void Configure(EntityTypeBuilder<T> builder)
            {
                builder.ToTable(_tableName).HasKey(p => p.Id);
            }

            public EntityConfiguration(string tableName)
            {
                _tableName = tableName;
            }
        }
        
        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            return base.SaveChanges();
        }
    }
}
