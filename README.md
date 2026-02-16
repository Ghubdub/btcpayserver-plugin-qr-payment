# QR Payment Plugin for BTCPay Server

A custom payment method plugin for BTCPay Server that displays a configurable QR code at checkout with a "Mark as Paid" button for manual settlement.

## Features

- 📱 **Custom QR Code Payment Method** - Displays at checkout alongside Bitcoin, Lightning, etc.
- ✏️ **Configurable Content** - Set custom title and description for the payment method
- 🔗 **Invoice-Specific QR Codes** - Each invoice gets a unique QR code
- ✅ **Mark as Paid Button** - Manually settle invoices after receiving payment
- 🎨 **Customizable Appearance** - Control title, description, and display order
- 🔒 **Secure API** - Requires authentication for payment operations

## Requirements

- BTCPay Server v2.0+
- .NET 8.0
- PostgreSQL database
- QRCoder NuGet package

## Installation

### 1. Clone and Build

```bash
# Clone the plugin repository
git clone https://github.com/yourusername/btcpay-qr-plugin.git
cd btcpay-qr-plugin

# Build the plugin
dotnet build
```

### 2. Deploy to BTCPay Server

Copy the built DLL to your BTCPay Server plugins directory:

```bash
# Copy the plugin DLL
cp bin/Debug/net8.0/BTCPayServer.Plugins.QRPayment.dll /path/to/btcpayserver/Plugins/

# Copy views and resources
cp -r Views/Shared/QRPayment* /path/to/btcpayserver/wwwroot/Plugins/QRPayment/
cp -r Resources/* /path/to/btcpayserver/wwwroot/Plugins/QRPayment/
```

### 3. Restart BTCPay Server

```bash
sudo systemctl restart btcpayserver
```

## Configuration

### Enable the Plugin

1. Log in to your BTCPay Server
2. Navigate to **Store Settings** → **Plugins**
3. Find "QR Payment Method" and enable it

### Configure Payment Method

1. Go to **Store Settings** → **QR Payment**
2. Configure the following:
   - **Title**: Display name (e.g., "Scan to Pay", "Custom QR")
   - **Description**: Instructions for customers
   - **Display Order**: Position in payment methods list
   - **Enable**: Check to activate the payment method

### API Configuration

The plugin exposes the following API endpoints:

```
GET  /api/v1/plugins/qr-payment/stores/{storeId}/config
PUT  /api/v1/plugins/qr-payment/stores/{storeId}/config
GET  /api/v1/plugins/qr-payment/invoices/{invoiceId}/qr
POST /api/v1/plugins/qr-payment/invoices/{invoiceId}/mark-paid
```

## API Usage

### Get Store Configuration

```bash
curl -X GET \
  -H "Authorization: token YOUR_API_KEY" \
  https://btcpay.example.com/api/v1/plugins/qr-payment/stores/{storeId}/config
```

### Update Store Configuration

```bash
curl -X PUT \
  -H "Authorization: token YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Scan to Pay",
    "content": "Use any payment app to scan this QR code",
    "isEnabled": true,
    "order": 5
  }' \
  https://btcpay.example.com/api/v1/plugins/qr-payment/stores/{storeId}/config
```

### Mark Invoice as Paid

```bash
curl -X POST \
  -H "Authorization: token YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{"notes": "Payment received via bank transfer"}' \
  https://btcpay.example.com/api/v1/plugins/qr-payment/invoices/{invoiceId}/mark-paid
```

## Plugin Structure

```
btcpay-qr-plugin/
├── BTCPayServer.Plugins.QRPayment.csproj    # Project file
├── QRPaymentPlugin.cs                        # Main plugin class
├── QRPaymentCheckoutMiddleware.cs            # Checkout UI middleware
├── QRPaymentInvoiceFilter.cs                 # Invoice event filter
├── Data/
│   ├── QRPaymentModels.cs                    # Database models
│   └── QRPaymentDbContext.cs                 # Entity Framework context
├── Services/
│   ├── QRPaymentService.cs                   # Business logic
│   └── QRCodeGeneratorService.cs             # QR code generation
├── Controllers/
│   ├── QRPaymentApiController.cs             # REST API
│   └── QRPaymentSettingsController.cs       # Settings UI
├── Views/
│   └── Shared/
│       ├── QRPaymentCheckout.cshtml          # Checkout template
│       └── QRPaymentSettings.cshtml          # Settings template
└── Resources/
    ├── qr-payment.css                        # Styles
    └── qr-payment.js                         # Client-side scripts
```

## Development

### Set Up Development Environment

1. Fork the BTCPay Server repository
2. Clone the plugin template
3. Add this plugin to the Plugins directory
4. Configure debugging in `appsettings.dev.json`

### Database Migrations

When updating the plugin, create migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## API Key Permissions

For API access, create an API key with the following permissions:
- `store.read` (for viewing configs)
- `store.modify` (for updating configs)
- `invoices.read` (for viewing invoice QR codes)
- `invoices.modify` (for marking as paid)

## Troubleshooting

### Plugin Not Appearing

1. Check BTCPay Server logs for errors
2. Verify the plugin DLL is in the correct directory
3. Ensure database migrations have run

### QR Code Not Displaying

1. Verify the payment method is enabled in store settings
2. Check that the invoice has a valid amount
3. Review browser console for JavaScript errors

### Mark as Paid Not Working

1. Verify API key has correct permissions
2. Check that the invoice exists and is not already paid
3. Review server logs for settlement errors

## Security Considerations

- API endpoints require authentication
- "Mark as Paid" should only be accessible to trusted users
- Consider implementing additional verification for high-value invoices
- All API calls are logged by BTCPay Server

## License

MIT License - See LICENSE file for details.

## Support

For issues and feature requests, please open a GitHub issue.
