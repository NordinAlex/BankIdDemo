using BankIdDemoApp.DTOs.BankId.Requests;
using BankIdDemoApp.DTOs.BankId.Responses;

namespace BankIdDemoApp.Services;

public interface IBankIdAuthService
{
    Task<BankIdAuthResponseDto> AuthenticateUserAsync(string endUserIp, string launchingType);
    Task<BankIdCollectResponseDto> CollectStatusAsync(string orderRef);
    Task CancelAsync(string orderRef);
}

public class BankIdAuthService : IBankIdAuthService
{
    private readonly IBankIdService _bankIdService;
    private readonly IQrCodeService _qrCodeService;
    private readonly ILogger<BankIdAuthService> _logger;

    public BankIdAuthService(IBankIdService bankIdService, IQrCodeService qrCodeService, ILogger<BankIdAuthService> logger)
    {
        _bankIdService = bankIdService;
        _qrCodeService = qrCodeService;
        _logger = logger;
    }

    public async Task<BankIdAuthResponseDto> AuthenticateUserAsync(string endUserIp, string launchingType)
    {
        BankIdAuthRequestDto request = new BankIdAuthRequestDto
        {
            EndUserIp = endUserIp,
            UserVisibleData = "IFRoaXMgaXMgYSBzYW1wbGUgdGV4dCB0byBiZSBzaWduZWQ=",
            ReturnRisk = true,
            Requirement = new RequirementDto
            {
                Risk = RiskLevel.Low,
            }
        };

        return await _bankIdService.AuthenticateAsync(request);
    }

    public async Task<BankIdCollectResponseDto> CollectStatusAsync(string orderRef)
    {
        return await _bankIdService.CollectAsync(orderRef);
    }

    public async Task CancelAsync(string orderRef)
    {
        await _bankIdService.CancelAsync(orderRef);
    }
}