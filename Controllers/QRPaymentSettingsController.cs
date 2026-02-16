using BTCPayServer.Plugins.QRPayment.Data;
using BTCPayServer.Plugins.QRPayment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BTCPayServer.Plugins.QRPayment.Controllers;

public class QRPaymentSettingsController : Controller
{
    private readonly QRPaymentService _qrPaymentService;

    public QRPaymentSettingsController(QRPaymentService qrPaymentService)
    {
        _qrPaymentService = qrPaymentService;
    }

    [HttpGet("/stores/{storeId}/settings/qrpayment")]
    [Authorize(AuthenticationSchemes = "Cookies")]
    public async Task<IActionResult> Index(string storeId)
    {
        var config = await _qrPaymentService.GetOrCreateStoreConfig(storeId);
        ViewBag.StoreId = storeId;
        ViewBag.Config = config;
        return View("QRPaymentSettings");
    }

    [HttpPost("/stores/{storeId}/settings/qrpayment")]
    [Authorize(AuthenticationSchemes = "Cookies")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQRPaymentConfig(string storeId, 
        [FromForm] string Title,
        [FromForm] string Content,
        [FromForm] int Order,
        [FromForm] bool IsEnabled = false)
    {
        var config = await _qrPaymentService.GetOrCreateStoreConfig(storeId);
        
        config.Title = Title;
        config.Content = Content;
        config.Order = Order;
        config.IsEnabled = IsEnabled;
        
        await _qrPaymentService.UpdateStoreConfig(config);
        
        TempData["Message"] = "QR Payment configuration saved successfully.";
        return RedirectToAction("Index", new { storeId });
    }
}
