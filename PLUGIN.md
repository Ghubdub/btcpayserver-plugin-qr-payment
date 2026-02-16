# QR Payment Plugin - Complete Documentation

## Overview

This BTCPay Server plugin adds a custom QR code payment method to your checkout page. Customers can scan the QR code with any payment app, and you can manually mark invoices as paid once payment is received.

## Quick Start

### 1. Install the Plugin

```bash
# Clone the plugin
git clone https://github.com/yourusername/btcpay-qr-plugin.git
cd btcpay-qr-plugin

# Build
dotnet build -c Release

# Copy to BTCPay Server plugins directory
cp bin/Release/net8.0/BTCPayServer.Plugins.QRPayment.dll /path/to/btcpayserver/Plugins/
cp -r Views/Resources /path/to/btcpayserver/wwwroot/Plugins/QRPayment/

# Restart BTCPay Server
sudo systemctl restart btcpayserver
```

### 2. Enable in Store Settings

1. Go to Store Settings → Plugins
2. Find "QR Payment Method" 
3. Click Enable

### 3. Configure the Payment Method

1. Navigate to Store Settings → QR Payment
2. Set Title (e.g., "Scan to Pay")
3. Set Description (e.g., "Scan this QR code with any payment app")
4. Set Display Order (lower = appears first)
5. Check "Enable QR Payment Method"
6. Click Save

## User Guide

### For Store Owners

#### Configuring the Payment Method

1. **Title**: This appears as the payment method name at checkout
   - Examples: "Scan to Pay", "Custom QR Payment", "Bank Transfer QR"

2. **Description**: Instructions shown to customers
   - Examples: "Scan this QR code with your banking app"
   - Maximum 1000 characters

3. **Display Order**: Controls position in payment list
   - 0 = first, 99 = last
   - Bitcoin = typically 0, Lightning = 1

4. **Enable**: Must be checked for the method to appear

#### Marking Invoices as Paid

When a customer pays via QR code (or any other method):

1. Locate the invoice in your BTCPay Server dashboard
2. Or use the "Mark as Paid" button directly on the checkout page
3. Confirm the payment
4. The invoice status updates to "Paid" and settles automatically

**API Usage:**

```bash
curl -X POST \
  -H "Authorization: token YOUR_API_KEY" \
  -d '{"notes": "Received via bank transfer"}' \
  https://btcpay.example.com/api/v1/plugins/qr-payment/invoices/Inv123/mark-paid
```

### For Customers

1. Select "QR Payment" at checkout
2. Scan the QR code with any payment app
3. Complete the payment in your app
4. The merchant will mark the invoice as paid

## Features

### ✅ Completed Features

1. **Custom QR Code Display**
   - Configurable title and description
   - Invoice-specific QR codes
   - Clean, responsive design

2. **Payment Method Integration**
   - Appears alongside Bitcoin/Lightning options
   - Respects display order settings
   - Enable/disable per store

3. **Mark as Paid Functionality**
   - One-click invoice settlement
   - Optional payment notes
   - Audit trail with timestamp and user

4. **API Endpoints**
   - RESTful API for all operations
   - Token-based authentication
   - Full CRUD for store configuration

### 🔄 Planned Features

- Multiple QR code templates
- Custom colors and branding
- Payment amount verification
- Webhook notifications
- QR code scan analytics

## Plugin Structure

```
btcpay-qr-plugin/
├── BTCPayServer.Plugins.QRPayment.csproj    # Project configuration
├── QRPaymentPlugin.cs                        # Plugin entry point
├── QRPaymentCheckoutMiddleware.cs            # Checkout UI injection
├── QRPaymentInvoiceFilter.cs                 # Invoice event handling
├── Data/
│   ├── QRPaymentModels.cs                   # Database entities
│   └── QRPaymentDbContext.cs                # Entity Framework setup
├── Services/
│   ├── QRPaymentService.cs                  # Business logic
│   └── QRCodeGeneratorService.cs            # QR code generation
├── Controllers/
│   ├── QRPaymentApiController.cs            # REST API endpoints
│   └── QRPaymentSettingsController.cs       # Settings UI
├── Views/
│   └── Shared/
│       ├── QRPaymentCheckout.cshtml         # Checkout template
│       └── QRPaymentSettings.cshtml         # Settings template
├── Resources/
│   ├── qr-payment.css                       # Styles
│   └── qr-payment.js                        # Client-side scripts
├── PluginSettings.json                      # Plugin metadata
└── README.md                                # Documentation
```

## API Reference

### Store Configuration

**Get Configuration**
```
GET /api/v1/plugins/qr-payment/stores/{storeId}/config
```

Response:
```json
{
  "id": "uuid",
  "storeId": "store123",
  "title": "Scan to Pay",
  "content": "Scan this QR code with your payment app",
  "isEnabled": true,
  "order": 5,
  "createdAt": "2026-02-16T02:00:00Z",
  "updatedAt": "2026-02-16T02:00:00Z"
}
```

**Update Configuration**
```
PUT /api/v1/plugins/qr-payment/stores/{storeId}/config
```

Request Body:
```json
{
  "title": "Custom QR Payment",
  "content": "Use any banking app to scan",
  "isEnabled": true,
  "order": 3
}
```

### Invoice Operations

**Get Invoice QR**
```
GET /api/v1/plugins/qr-payment/invoices/{invoiceId}/qr
```

**Mark as Paid**
```
POST /api/v1/plugins/qr-payment/invoices/{invoiceId}/mark-paid
```

Request Body:
```json
{
  "notes": "Payment received and verified"
}
```

Response:
```json
{
  "invoiceId": "Inv123",
  "isPaid": true,
  "paidAt": "2026-02-16T02:30:00Z",
  "paidBy": "user123",
  "paymentNotes": "Payment received and verified",
  "message": "Invoice marked as paid and settled"
}
```

## Error Handling

### Common Errors

| Error Code | Message | Solution |
|------------|---------|----------|
| 400 | Invoice not found or already paid | Check invoice ID |
| 401 | Unauthorized | Add valid API token |
| 404 | Store config not found | Enable plugin in settings |
| 500 | Settlement failed | Check BTCPay Server logs |

### Error Response Format

```json
{
  "error": "Descriptive error message"
}
```

## Troubleshooting

### QR Code Not Showing

1. Verify plugin is enabled in Store Settings
2. Check browser console for JavaScript errors
3. Ensure no ad blocker is interfering
4. Try a different browser

### Mark as Paid Fails

1. Verify API key has `invoices.modify` permission
2. Check the invoice exists and is not already paid
3. Review BTCPay Server logs
4. Ensure the invoice is in a valid state (cannot pay settled invoices)

### Plugin Doesn't Appear

1. Check BTCPay Server logs for loading errors
2. Verify plugin DLL is in the correct location
3. Ensure all dependencies are installed
4. Restart BTCPay Server after installation

## Security Best Practices

1. **API Keys**: Use dedicated API keys with minimal permissions
2. **Access Control**: Limit "Mark as Paid" access to trusted users
3. **Verification**: Always verify payment before marking as paid
4. **Monitoring**: Review audit logs regularly
5. **HTTPS**: Ensure all API calls use HTTPS in production

## Database

The plugin creates two tables:

```sql
-- Store configurations
CREATE TABLE qrbayment_store_configs (
    id VARCHAR(255) PRIMARY KEY,
    store_id VARCHAR(255) NOT NULL,
    title VARCHAR(200),
    content VARCHAR(1000),
    is_enabled BOOLEAN DEFAULT FALSE,
    order_index INTEGER DEFAULT 99,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Invoice records
CREATE TABLE qrbayment_invoices (
    id VARCHAR(255) PRIMARY KEY,
    invoice_id VARCHAR(100) NOT NULL,
    store_id VARCHAR(255) NOT NULL,
    qr_code_content VARCHAR(2000),
    is_paid BOOLEAN DEFAULT FALSE,
    paid_at TIMESTAMP,
    paid_by VARCHAR(100),
    payment_notes VARCHAR(500),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

MIT License - See LICENSE file for details.

## Support

- GitHub Issues: Report bugs and request features
- BTCPay Server Discord: Community support
- Documentation: See README.md for detailed guides
