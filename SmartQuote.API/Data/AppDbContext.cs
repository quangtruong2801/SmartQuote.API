using Microsoft.EntityFrameworkCore;
using SmartQuote.API.Entities;

namespace SmartQuote.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Material> Materials => Set<Material>();
        public DbSet<ProductTemplate> ProductTemplates => Set<ProductTemplate>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Quotation> Quotations => Set<Quotation>();
        public DbSet<QuotationItem> QuotationItems => Set<QuotationItem>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- USER ----------
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username)
                      .IsUnique();

                entity.Property(u => u.Username)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(u => u.PasswordHash)
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(u => u.Role)
                      .HasMaxLength(20)
                      .IsRequired();
            });

            // ---------- QUOTATION ----------
            modelBuilder.Entity<Quotation>(entity =>
            {
                entity.HasMany(q => q.Items)
                      .WithOne(i => i.Quotation)
                      .HasForeignKey(i => i.QuotationId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(q => q.Status)
                      .HasConversion<string>();
            });

            // ---------- POSTGRES NAMING ----------
            //foreach (var entity in modelBuilder.Model.GetEntityTypes())
            //{
            //    entity.SetTableName(
            //        entity.GetTableName()!.ToLower()
            //    );
            //}
        }
    }
}
