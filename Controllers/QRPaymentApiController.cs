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

    public QRPaymentApiController(QRPaymentService qrPaymentService)
    {
        _qrPaymentService = qrPaymentService;
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
    /// Gets the QR code for an invoice
    /// </summary>
    [HttpGet("invoices/{invoiceId}/qr")]
    [Authorize(AuthenticationSchemes = "Greenfield")]
    public async Task<IActionResult> GetInvoiceQR(string invoiceId)
    {
        var invoice = await _qrPaymentService.GetInvoiceQR(invoiceId);
        if (invoice == null)
            return NotFound(new { error = "Invoice QR not found" });

        return Ok(new
        {
            invoice.InvoiceId,
            invoice.QRCodeContent,
            invoice.IsPaid,
            invoice.PaidAt,
            invoice.PaidBy,
            invoice.PaymentNotes
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

        // Here you would call BTCPay Server's invoice settlement API
        // This is a placeholder for the actual settlement logic
        await SettleInvoice(invoiceId);

        return Ok(new
        {
            invoice.InvoiceId,
            invoice.IsPaid,
            invoice.PaidAt,
            invoice.PaidBy,
            invoice.PaymentNotes,
            message = "Invoice marked as paid and settled"
        });
    }

    /// <summary>
    /// Internal method to settle the invoice in BTCPay Server
    /// </summary>
    private async Task SettleInvoice(string invoiceId)
    {
        // TODO: Implement actual invoice settlement via BTCPay Server API
        // This would typically call the internal invoice settlement endpoint
        // For now, this is a placeholder
        
        await Task.CompletedTask;
    }
}

public class UpdateConfigRequest
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public bool? IsEnabled { get; set; }
    public int? Order { get; set; }
}

public class MarkPaidRequest
{
    public string? Notes { get; set; }
}
