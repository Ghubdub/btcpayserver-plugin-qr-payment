using BTCPayServer.Plugins.Mvc;
using BTCPayServer.Plugins.QRPayment.Services;
using Microsoft.AspNetCore.Http.Extensions;
using BTCPayServer.Plugins.Mvc.Checkout;

namespace BTCPayServer.Plugins.QRPayment;

public class QRPaymentCheckoutMiddleware : IPaymentMethodCheckoutMiddleware
{
    private readonly QRPaymentService _qrPaymentService;
    private readonly QRCodeGeneratorService _qrCodeGenerator;

    public QRPaymentCheckoutMiddleware(
        QRPaymentService qrPaymentService,
        QRCodeGeneratorService qrCodeGenerator)
    {
        _qrPaymentService = qrPaymentService;
        _qrCodeGenerator = qrCodeGenerator;
    }

    public async Task<CheckoutUIState> Execute(CheckoutContext context)
    {
        var storeConfig = await _qrPaymentService.GetEnabledStoreConfig(context.StoreId);
        if (storeConfig == null)
        {
            return CheckoutUIState.Create(context.PaymentMethodId, false);
        }

        // Generate invoice-specific QR code content
        var invoice = context.Invoice;
        var qrContent = $"{{\"invoice\":\"{invoice.Id}\",\"amount\":\"{invoice.TotalFormatted}\",\"currency\":\"{invoice.Currency}\",\"store\":\"{context.StoreName}\"}}";
        
        // Store QR content for this invoice
        await _qrPaymentService.CreateInvoiceQR(invoice.Id, context.StoreId, qrContent);

        // Generate QR code image
        var qrCodeImage = _qrCodeGenerator.GenerateQRCode(qrContent, 250);

        // Add view data for the checkout UI
        context.SetAdditionalData("QRPayment", new
        {
            Enabled = true,
            Title = storeConfig.Title,
            Content = storeConfig.Content,
            QRCodeImage = qrCodeImage,
            InvoiceId = invoice.Id,
            Amount = invoice.TotalFormatted,
            Currency = invoice.Currency
        });

        return CheckoutUIState.Create(context.PaymentMethodId, true);
    }
}
