using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTCPayServer.Plugins.QRPayment.Data;

/// <summary>
/// Store-level QR payment configuration
/// </summary>
public class QRPaymentStoreConfig
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    public string StoreId { get; set; } = string.Empty;
    
    /// <summary>
    /// Title displayed on the checkout page
    /// </summary>
    [MaxLength(200)]
    public string? Title { get; set; }
    
    /// <summary>
    /// Content/Description of the QR code payment
    /// </summary>
    [MaxLength(1000)]
    public string? Content { get; set; }
    
    /// <summary>
    /// Whether this payment method is enabled
    /// </summary>
    public bool IsEnabled { get; set; }
    
    /// <summary>
    /// Display order in payment methods list
    /// </summary>
    public int Order { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Invoice-specific QR payment record
/// </summary>
public class QRPaymentInvoice
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [MaxLength(100)]
    public string InvoiceId { get; set; } = string.Empty;
    
    [Required]
    public string StoreId { get; set; } = string.Empty;
    
    /// <summary>
    /// QR code content generated for this invoice
    /// </summary>
    [MaxLength(2000)]
    public string? QRCodeContent { get; set; }
    
    /// <summary>
    /// Whether the invoice has been marked as paid
    /// </summary>
    public bool IsPaid { get; set; }
    
    /// <summary>
    /// When the payment was marked as paid
    /// </summary>
    public DateTime? PaidAt { get; set; }
    
    /// <summary>
    /// Who marked it as paid (user ID)
    /// </summary>
    [MaxLength(100)]
    public string? PaidBy { get; set; }
    
    /// <summary>
    /// Any notes about the payment
    /// </summary>
    [MaxLength(500)]
    public string? PaymentNotes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
