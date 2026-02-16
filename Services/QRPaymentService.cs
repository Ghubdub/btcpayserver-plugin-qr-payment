using BTCPayServer.Plugins.QRPayment.Data;
using Microsoft.EntityFrameworkCore;

namespace BTCPayServer.Plugins.QRPayment.Services;

public class QRPaymentService
{
    private readonly QRPaymentDbContext _context;
    private readonly QRCodeGeneratorService _qrGenerator;

    public QRPaymentService(QRPaymentDbContext context, QRCodeGeneratorService qrGenerator)
    {
        _context = context;
        _qrGenerator = qrGenerator;
    }

    /// <summary>
    /// Gets or creates the store configuration for QR payments
    /// </summary>
    public async Task<QRPaymentStoreConfig> GetOrCreateStoreConfig(string storeId)
    {
        var config = await _context.QRPaymentStoreConfigs
            .FirstOrDefaultAsync(c => c.StoreId == storeId);

        if (config == null)
        {
            config = new QRPaymentStoreConfig
            {
                StoreId = storeId,
                Title = "QR Payment",
                Content = "Scan this QR code to pay",
                IsEnabled = false,
                Order = 99
            };
            _context.QRPaymentStoreConfigs.Add(config);
            await _context.SaveChangesAsync();
        }

        return config;
    }

    /// <summary>
    /// Updates the store configuration
    /// </summary>
    public async Task UpdateStoreConfig(QRPaymentStoreConfig config)
    {
        config.UpdatedAt = DateTime.UtcNow;
        _context.QRPaymentStoreConfigs.Update(config);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets the store configuration if enabled
    /// </summary>
    public async Task<QRPaymentStoreConfig?> GetEnabledStoreConfig(string storeId)
    {
        return await _context.QRPaymentStoreConfigs
            .FirstOrDefaultAsync(c => c.StoreId == storeId && c.IsEnabled);
    }

    /// <summary>
    /// Creates or updates an invoice record with QR code content
    /// </summary>
    public async Task<QRPaymentInvoice> CreateInvoiceQR(string invoiceId, string storeId, string qrContent)
    {
        var invoice = await _context.QRPaymentInvoices
            .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

        if (invoice == null)
        {
            invoice = new QRPaymentInvoice
            {
                InvoiceId = invoiceId,
                StoreId = storeId,
                QRCodeContent = qrContent
            };
            _context.QRPaymentInvoices.Add(invoice);
        }
        else
        {
            invoice.QRCodeContent = qrContent;
        }

        await _context.SaveChangesAsync();
        return invoice;
    }

    /// <summary>
    /// Gets the invoice QR record
    /// </summary>
    public async Task<QRPaymentInvoice?> GetInvoiceQR(string invoiceId)
    {
        return await _context.QRPaymentInvoices
            .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
    }

    /// <summary>
    /// Marks an invoice as paid
    /// </summary>
    public async Task<QRPaymentInvoice?> MarkAsPaid(string invoiceId, string userId, string? notes = null)
    {
        var invoice = await _context.QRPaymentInvoices
            .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId && !i.IsPaid);

        if (invoice != null)
        {
            invoice.IsPaid = true;
            invoice.PaidAt = DateTime.UtcNow;
            invoice.PaidBy = userId;
            invoice.PaymentNotes = notes;
            await _context.SaveChangesAsync();
        }

        return invoice;
    }

    /// <summary>
    /// Generates QR code data URL for display
    /// </summary>
    public string GenerateDisplayQRCode(string content)
    {
        return _qrGenerator.GenerateQRCode(content, 250);
    }
}
