using Microsoft.EntityFrameworkCore;

namespace BTCPayServer.Plugins.QRPayment.Data;

public class QRPaymentDbContext : DbContext
{
    public QRPaymentDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<QRPaymentStoreConfig> QRPaymentStoreConfigs { get; set; } = null!;
    public DbSet<QRPaymentInvoice> QRPaymentInvoices { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<QRPaymentStoreConfig>(entity =>
        {
            entity.ToTable("qrpayment_store_configs");
            entity.HasIndex(e => e.StoreId);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Content).HasMaxLength(1000);
        });

        modelBuilder.Entity<QRPaymentInvoice>(entity =>
        {
            entity.ToTable("qrpayment_invoices");
            entity.HasIndex(e => e.InvoiceId);
            entity.HasIndex(e => e.StoreId);
            entity.Property(e => e.QRCodeContent).HasMaxLength(2000);
            entity.Property(e => e.PaymentNotes).HasMaxLength(500);
        });
    }
}
