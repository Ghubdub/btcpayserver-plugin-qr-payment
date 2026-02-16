using BTCPayServer.Abstractions.Contracts;
using BTCPayServer.Plugins.QRPayment.Services;

namespace BTCPayServer.Plugins.QRPayment;

public class QRPaymentInvoiceFilter : IPluginHookFilter
{
    public string Hook => "InvoiceStatusChanged";

    public async Task<object> Execute(object? context, IDictionary<string, object?> settings)
    {
        // Handle invoice status changes if needed
        return true;
    }
}
