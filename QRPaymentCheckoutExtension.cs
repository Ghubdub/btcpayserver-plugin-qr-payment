using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Plugins.QRPayment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BTCPayServer.Plugins.QRPayment;

public class QRPaymentCheckoutExtension : IUIExtensionPoint
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly QRPaymentService _qrPaymentService;

    public QRPaymentCheckoutExtension(
        IHttpContextAccessor httpContextAccessor,
        QRPaymentService qrPaymentService)
    {
        _httpContextAccessor = httpContextAccessor;
        _qrPaymentService = qrPaymentService;
    }

    public string Location => "checkout-payment-method";

    public async Task<string> InvokeAsync(string location, object? model)
    {
        if (location != "checkout-payment-method")
            return string.Empty;

        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return string.Empty;

            // Get invoice ID from route or query
            var invoiceId = httpContext.Request.RouteValues["invoiceId"]?.ToString() 
                ?? httpContext.Request.Query["invoiceId"].ToString();

            // Use View Component directly
            var viewComponent = new QRPaymentCheckoutViewComponent(_qrPaymentService);
            var result = await viewComponent.InvokeAsync(invoiceId);
            
            // Render the view component
            using var sw = new StringWriter();
            var viewContext = new ViewContext(
                new Microsoft.AspNetCore.Mvc.Rendering.ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.ModelBinding.ActionDescriptor()),
                new Microsoft.AspNetCore.Mvc.ViewFeatures.StringWriter(viewContext: null),
                new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary(),
                new Dictionary<string, string>(),
                sw,
                new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper());
            
            // Simplified approach - return a simple button that triggers our modal
            return $@"
<div class=""qr-payment-checkout"">
    <button type=""button"" 
            class=""btcpay-pill m-0 payment-method qr-payment-pill"" 
            data-toggle=""modal"" 
            data-target=""#qr-payment-modal""
            data-invoice=""{invoiceId}"">
        <span>📱</span>
        <span>QR Payment</span>
    </button>
</div>";
        }
        catch (Exception ex)
        {
            return $@"<!-- QR Payment Error: {ex.Message} -->";
        }
    }
}
