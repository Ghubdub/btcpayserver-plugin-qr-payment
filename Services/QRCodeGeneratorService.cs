using BTCPayServer.Plugins.QRPayment.Data;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace BTCPayServer.Plugins.QRPayment.Services;

public class QRCodeGeneratorService
{
    /// <summary>
    /// Generates a QR code as PNG data URL
    /// </summary>
    public string GenerateQRCode(string content, int size = 200)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(size);
        
        return $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
    }

    /// <summary>
    /// Generates a QR code with custom colors
    /// </summary>
    public string GenerateQRCode(string content, string darkColor, string lightColor, int size = 200)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        
        var dark = System.Drawing.ColorTranslator.FromHtml(darkColor);
        var light = System.Drawing.ColorTranslator.FromHtml(lightColor);
        
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(size, dark, light);
        
        return $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
    }
}
