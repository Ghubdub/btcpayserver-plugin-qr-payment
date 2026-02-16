using BTCPayServer.Plugins.Mvc;
using BTCPayServer.Plugins.QRPayment.Data;
using BTCPayServer.Plugins.QRPayment.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BTCPayServer.Plugins.QRPayment;

public class QRPaymentPlugin : BasePlugin
{
    public override string Identifier => "BTCPayServer.Plugins.QRPayment";
    public override string DisplayName => "QR Payment Method";
    public override string Description => "Custom QR code payment method with API for manual invoice settlement";

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
        base.Configure(pages);
    }
}
