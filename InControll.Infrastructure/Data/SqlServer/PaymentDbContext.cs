using InControll.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InControll.Infrastrucuture;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }
    
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>(builder =>
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(p => p.Currency).HasMaxLength(3).IsRequired(); //e.g.: "BRL", "USD"
            builder.Property(p => p.Status).HasConversion<string>();
            builder.Property(p => p.CustomerId).HasMaxLength(100).IsRequired();
            builder.Property(p => p.OrderId).HasMaxLength(100).IsRequired();
            
            //one-many relationship (Payment to Transactions)
            builder.HasMany(p => p.Transactions).WithOne().HasForeignKey(t => t.PaymentId).IsRequired();
            
            builder.Metadata.FindNavigation(nameof(Payment.Transactions))?.SetPropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Transaction>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(t => t.Type).HasConversion<string>();
            builder.Property(t => t.GatewayTransactionId).HasMaxLength(250).IsRequired(false);
            builder.Property(t => t.GatewayResponse).HasColumnType("nvarchar(max)").IsRequired().IsRequired();
            builder.Property(t => t.Description).HasMaxLength(500).IsRequired(false);
        });
    }
}