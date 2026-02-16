using BTCPayServer.Plugins.QRPayment.Data;
using BTCPayServer.Plugins.QRPayment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BTCPayServer.Plugins.QRPayment.Controllers;

[ApiController]
[Route("api/v1/plugins/qr-payment")]
public class QRPaymentApiController : ControllerBase
{
    private readonly QRPaymentService _qrPaymentService;
    private readonly QRCodeGeneratorService _qrCodeGenerator;

    public QRPaymentApiController(QRPaymentService qrPaymentService, QRCodeGeneratorService qrCodeGenerator)
    {
        _qrPaymentService = qrPaymentService;
        _qrCodeGenerator = qrCodeGenerator;
    }

    /// <summary>
    /// Gets the store configuration for QR payments
    /// </summary>
    [HttpGet("stores/{storeId}/config")]
    [Authorize(AuthenticationSchemes = "Greenfield")]
    public async Task<IActionResult> GetStoreConfig(string storeId)
    {
        var config = await _qrPaymentService.GetOrCreateStoreConfig(storeId);
        return Ok(new
        {
            config.Id,
            config.StoreId,
            config.Title,
            config.Content,
            config.IsEnabled,
            config.Order,
            config.CreatedAt,
            config.UpdatedAt
        });
    }

    /// <summary>
    /// Updates the store configuration for QR payments
    /// </summary>
    [HttpPut("stores/{storeId}/config")]
    [Authorize(AuthenticationSchemes = "Greenfield")]
    public async Task<IActionResult> UpdateStoreConfig(string storeId, [FromBody] UpdateConfigRequest request)
    {
        var config = await _qrPaymentService.GetOrCreateStoreConfig(storeId);
        
        if (request.Title != null)
            config.Title = request.Title;
        if (request.Content != null)
            config.Content = request.Content;
        if (request.IsEnabled.HasValue)
            config.IsEnabled = request.IsEnabled.Value;
        if (request.Order.HasValue)
            config.Order = request.Order.Value;

        await _qrPaymentService.UpdateStoreConfig(config);
        
        return Ok(new
        {
            config.Id,
            config.StoreId,
            config.Title,
            config.Content,
            config.IsEnabled,
            config.Order
        });
    }

    /// <summary>
    /// Generate a QR code for a payment
    /// </summary>
    [HttpPost("stores/{storeId}/generate-qr")]
    [Authorize(AuthenticationSchemes = "Greenfield")]
    public IActionResult GenerateQRCode(string storeId, [FromBody] GenerateQRRequest request)
    {
        var qrContent = $"{{\"invoice\":\"{request.InvoiceId}\",\"amount\":\"{request.Amount}\",\"currency\":\"{request.Currency}\",\"note\":\"{request.Note ?? ""}\"}}";
        
        var qrCodeImage = _qrCodeGenerator.GenerateQRCode(qrContent, 250);
        
        return Ok(new
        {
            qrCodeImage,
            content = qrContent
        });
    }

    /// <summary>
    /// Marks an invoice as paid (settles the invoice)
    /// </summary>
    [HttpPost("invoices/{invoiceId}/mark-paid")]
    [Authorize(AuthenticationSchemes = "Greenfield")]
    public async Task<IActionResult> MarkAsPaid(string invoiceId, [FromBody] MarkPaidRequest? request = null)
    {
        var userId = User.FindFirst("userId")?.Value ?? "api";
        
        var invoice = await _qrPaymentService.MarkAsPaid(invoiceId, userId, request?.Notes);
        
        if (invoice == null)
            return NotFound(new { error = "Invoice not found or already paid" });

        return Ok(new
        {
            invoice.InvoiceId,
            invoice.IsPaid,
            invoice.PaidAt,
            invoice.PaidBy,
            invoice.PaymentNotes,
            message = "Invoice marked as paid"
        });
    }

    /// <summary>
    /// Gets payment status for an invoice
    /// </summary>
    [HttpGet("invoices/{invoiceId}/status")]
    [Authorize(AuthenticationSchemes = "Greenfield")]
    public async Task<IActionResult> GetInvoiceStatus(string invoiceId)
    {
        var invoice = await _qrPaymentService.GetInvoiceQR(invoiceId);
        if (invoice == null)
            return NotFound(new { error = "Invoice not found" });

        return Ok(new
        {
            invoice.InvoiceId,
            invoice.IsPaid,
            invoice.PaidAt,
            invoice.PaidBy,
            invoice.PaymentNotes
        });
    }
}

public class UpdateConfigRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public bool? IsEnabled { get; set; }
    public int? Order { get; set; }
}

public class GenerateQRRequest
{
    public string InvoiceId { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public string? Note { get; set; }
}

public class MarkPaidRequest
{
    public string? Notes { get; set; }
}
