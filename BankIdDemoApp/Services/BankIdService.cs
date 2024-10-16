using BankIdDemoApp.DTOs.BankId.Requests;
using BankIdDemoApp.DTOs.BankId.Responses;
namespace BankIdDemoApp.Services;

public interface IBankIdService
{
    Task<BankIdAuthResponseDto> AuthenticateAsync(BankIdAuthRequestDto request);
    Task<BankIdCollectResponseDto> CollectAsync(string orderRef);
    Task CancelAsync(string orderRef);
}

public class BankIdService : IBankIdService
{
    private readonly IBankIdHttpClient _bankIdHttpClient;
    private readonly ILogger<BankIdService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _apiVersion;

    public BankIdService(IBankIdHttpClient bankIdHttpClient, ILogger<BankIdService> logger, IConfiguration configuration)
    {
        _bankIdHttpClient = bankIdHttpClient;
        _logger = logger;
        _configuration = configuration;
        _apiVersion = _configuration["BankID:ApiVersion"] ?? throw new ArgumentNullException("ApiVersion is missing in configuration");
    }

    public async Task<BankIdAuthResponseDto> AuthenticateAsync(BankIdAuthRequestDto request)
    {
        try
        {
            var url = $"{_apiVersion}/auth";
            var response = await _bankIdHttpClient.PostAsync(url, request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to authenticate: {response.StatusCode}");
                var errors = await response.Content.ReadFromJsonAsync<BankIdErrorResponseDto>();
                throw new HttpRequestException($"Error during authentication: {errors?.Details}");
            }

            return await response.Content.ReadFromJsonAsync<BankIdAuthResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during authentication: {ex.Message}");
            throw;
        }
    }

    public async Task<BankIdCollectResponseDto> CollectAsync(string orderRef)
    {
        try
        {
            var url = $"{_apiVersion}/collect";
            var request = new { orderRef };
            var response = await _bankIdHttpClient.PostAsync(url, request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to collect: {response.StatusCode}");
                throw new HttpRequestException($"Error during collect: {response.StatusCode}");
            }

            return await response.Content.ReadFromJsonAsync<BankIdCollectResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during collect: {ex.Message}");
            throw;
        }
    }

    public async Task CancelAsync(string orderRef)
    {
        try
        {
            var url = $"{_apiVersion}/cancel";
            var request = new { orderRef };
            var response = await _bankIdHttpClient.PostAsync(url, request);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponse(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during cancel: {ex.Message}");
            throw;
        }
    }

    private async Task HandleErrorResponse(HttpResponseMessage response)
    {
        var statusCode = (int)response.StatusCode;
        var errorResponse = await response.Content.ReadAsStringAsync();

        _logger.LogError($"Error {statusCode}: {errorResponse}");
        switch (statusCode)
        {
            case 400: throw new ArgumentException("Bad request");
            case 401: throw new UnauthorizedAccessException("Unauthorized");
            case 403: throw new UnauthorizedAccessException("Forbidden");
            case 404: throw new Exception("Not found");
            case 500: throw new Exception("Internal server error");
            case 503: throw new Exception("Service unavailable");
            default: throw new Exception("Unexpected error");
        }
    }
}
