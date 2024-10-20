using BankIdDemoApp.DTOs.BankId.Responses;
using BankIdDemoApp.Models.BankId;
using QRCoder;
using System.Security.Cryptography;
using System.Text;

namespace BankIdDemoApp.Services;

public interface IQrCodeService
{
    Task<string> GenerateQrCodeAsync(BankIdAuthQrResponse qrData);
}

public class QrCodeService : IQrCodeService
{
    public async Task<string> GenerateQrCodeAsync(BankIdAuthQrResponse qrResponse)
    {
        var timeInSeconds = (int)(DateTime.UtcNow - qrResponse.orderTime).TotalSeconds;
        var qrAuthCode = await ComputeHmacSha256Async(qrResponse.qrStartSecret, timeInSeconds.ToString());
        var qrData = $"bankid.{qrResponse.qrStartToken}.{timeInSeconds}.{qrAuthCode}";

        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q))
        using (Base64QRCode qrCode = new Base64QRCode(qrCodeData))
        {
            // Generera QR-koden som en base64-sträng.
            return qrCode.GetGraphic(20);
        }
    }

    private async Task<string> ComputeHmacSha256Async(string secret, string message)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
        {
            var hash = await Task.Run(() => hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
