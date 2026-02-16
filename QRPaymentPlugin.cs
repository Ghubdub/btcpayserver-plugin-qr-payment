using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Plugins.QRPayment.Data;
using BTCPayServer.Plugins.QRPayment.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace BTCPayServer.Plugins.QRPayment;

public class QRPaymentPlugin : BasePlugin
{
    public override string Identifier => "BTCPayServer.Plugins.QRPayment";
    public override string DisplayName => "QR Payment Method";
    public override string Description => "Custom QR code payment method at checkout with manual settlement";

    public override void Execute(IServiceCollection services)
    {
        // Register database context
        services.AddDbContext<QRPaymentDbContext>((provider, options) =>
        {
            var dbContextOptions = provider.GetRequiredService<DbContextOptions>();
            options.UseNpgsql(dbContextOptions);
        });

        // Register services
        services.AddSingleton<QRPaymentService>();
        services.AddSingleton<QRCodeGeneratorService>();

        // Register View Component
        services.AddTransient<QRPaymentCheckoutViewComponent>();

        base.Execute(services);
    }

    public override void Configure(Func<string, Microsoft.AspNetCore.Mvc.RazorPages.PageConventionCollection> pages)
    {
        // Register UI extension point for checkout
        services.AddSingleton<IUIExtensionPoint, QRPaymentCheckoutExtension>();
        
        base.Configure(pages);
    }
}

// View Component for checkout
public class QRPaymentCheckoutViewComponent : ViewComponent
{
    private readonly QRPaymentService _qrPaymentService;

    public QRPaymentCheckoutViewComponent(QRPaymentService qrPaymentService)
    {
        _qrPaymentService = qrPaymentService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string? invoiceId = null)
    {
        // Get store ID - simplified for now
        var storeId = "default";
        var config = await _qrPaymentService.GetEnabledStoreConfig(storeId);
        
        var model = new QRPaymentCheckoutModel
        {
            IsEnabled = config?.IsEnabled ?? false,
            Title = config?.Title ?? "QR Payment",
            Content = config?.Content ?? "Scan to pay",
            InvoiceId = invoiceId ?? ""
        };

        return View(model);
    }
}

public class QRPaymentCheckoutModel
{
    public bool IsEnabled { get; set; }
    public string Title { get; set; } = "QR Payment";
    public string Content { get; set; } = "Scan to pay";
    public string InvoiceId { get; set; } = string.Empty;
}
