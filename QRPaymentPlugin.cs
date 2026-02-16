using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Abstractions.Models;
using BTCPayServer.Plugins.QRPayment.Data;
using BTCPayServer.Plugins.QRPayment.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BTCPayServer.Plugins.QRPayment;

public class QRPaymentPlugin : BasePlugin
{
    public override string Identifier => "BTCPayServer.Plugins.QRPayment";
    public override string DisplayName => "QR Payment Method";
    public override string Description => "Custom QR code payment method with configurable content";

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

        base.Execute(services);
    }

    public override void Configure(Func<string, Microsoft.AspNetCore.Mvc.RazorPages.PageConventionCollection> pages)
    {
        // Add custom payment method to checkout
        services.AddTransient<IPaymentMethodCheckoutMiddleware, QRPaymentCheckoutMiddleware>();
        
        base.Configure(pages);
    }

    public override void PreRegister(IEnumerable<IPluginHookFilter> filters)
    {
        filters.Add(new QRPaymentInvoiceFilter());
        base.PreRegister(filters);
    }
}
