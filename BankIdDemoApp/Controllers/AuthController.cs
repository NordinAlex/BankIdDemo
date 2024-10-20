using BankIdDemoApp.Extensions;
using BankIdDemoApp.Models.BankId;
using BankIdDemoApp.Services;
using BankIdDemoApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BankIdDemoApp.Controllers;
public class AuthController(IBankIdAuthService bankIdAuthService, ISessionService sessionService, IQrCodeService qrCodeService, ILogger<AuthController> logger) : Controller
{
    private readonly IBankIdAuthService _bankIdAuthService = bankIdAuthService;
    private readonly ISessionService _sessionService = sessionService;
    private readonly IQrCodeService _qrCodeService = qrCodeService;
    private readonly ILogger<AuthController> _logger = logger;


    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Authenticate(string endUserIp, string launchingType)
    {
        try
        {
            // Starta autentisering och hämta svar
            var response = await _bankIdAuthService.AuthenticateUserAsync(endUserIp, launchingType);

            // Spara QR-data och OrderRef i sessionen
            var qrResponse = new BankIdAuthQrResponse
            {
                qrStartToken = response.QrStartToken,
                qrStartSecret = response.QrStartSecret,
                orderTime = DateTime.UtcNow
            };
            _sessionService.StoreQrResponse(qrResponse);
            _sessionService.StoreOrderRef(response.OrderRef);

            // Skapa ViewModel och generera QR-koden om nödvändigt
            var viewModel = new BankIdAuthViewModel
            {
                AutoStartToken = response.AutoStartToken,
                LaunchingType = launchingType,
                ReturnUrl = Url.Action("Index", "MinaSidor", null, Request.Scheme),
                
            };

            if (launchingType == "BankIDQrCode")
            {
                viewModel.QrCodeBase64 = await _qrCodeService.GenerateQrCodeAsync(qrResponse);
            }

            return View("Authenticate", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ett fel inträffade vid autentisering: {ex.Message}");
            ViewData["ErrorMessage"] = "Ett fel inträffade vid autentiseringen. Försök igen.";
            return View("Login");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQrCode()
    {
        try
        {
            var qrResponse = _sessionService.GetQrResponse();
            if (qrResponse == null)
            {
                return BadRequest(new { status = "error", message = "QR-data saknas. Vänligen försök igen." });
            }

            var qrCodeBase64 = await _qrCodeService.GenerateQrCodeAsync(qrResponse);
            return Ok(new { qrCodeBase64 });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ett fel inträffade i UpdateQrCode: {ex.Message}");
            return StatusCode(500, new { status = "error", message = "Ett internt fel inträffade. Vänligen försök igen senare." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CollectStatus()
    {
        try
        {
            var orderRef = _sessionService.GetOrderRef();
            if (string.IsNullOrEmpty(orderRef))
            {
                return BadRequest(new { status = "error", message = "OrderRef saknas." });
            }

            var collectData = await _bankIdAuthService.CollectStatusAsync(orderRef);
          

            if (collectData == null)
            {
                return BadRequest(new { status = "error", message = "Kunde inte tolka svaret från BankID." });
            }

            if (collectData.Status == "complete" && collectData.CompletionData != null)
            {
                var completionDataJson = JsonSerializer.Serialize(collectData.CompletionData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                _sessionService.StoreCompletionData(completionDataJson);
                return Ok(new { status = "complete", collectData.HintCode });
            }
            else if (collectData.Status == "pending")
            {
                var partialViewHtml = await this.RenderViewAsync("_CollectStatus", collectData, true);
                return Ok(new { status = "pending", collectData.HintCode, partialViewHtml });
            }
            else if (collectData.Status == "failed")
            {
                var partialViewHtml = await this.RenderViewAsync("_CollectStatus", collectData, true);
                return Ok(new { status = "failed", partialViewHtml });
            }

            return BadRequest(new { status = "error", message = "Okänt statusvärde." });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ett fel inträffade i CollectStatus: {ex.Message}");
            return StatusCode(500, new { status = "error", message = "Ett internt fel inträffade. Vänligen försök igen senare." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Cancel()
    {
        try
        {
            var orderRef = _sessionService.GetOrderRef();
            if (string.IsNullOrEmpty(orderRef))
            {
                return BadRequest(new { status = "error", message = "OrderRef saknas." });
            }

            await _bankIdAuthService.CancelAsync(orderRef);
            return Ok(new { status = "success" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ett fel inträffade i Cancel: {ex.Message}");
            return StatusCode(500, new { status = "error", message = "Ett internt fel inträffade. Vänligen försök igen senare." });
        }
    }

}
