using System.Net.Http.Headers;
using System.Text;

namespace BankIdDemoApp.Services;

public interface IBankIdHttpClient
{
    Task<HttpResponseMessage> PostAsync(string url, object request);
}

public class BankIdHttpClient : IBankIdHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BankIdHttpClient> _logger;
    private readonly IJsonSerializeService _jsonSerializeService;

    public BankIdHttpClient(HttpClient httpClient, ILogger<BankIdHttpClient> logger, IJsonSerializeService jsonSerializeService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonSerializeService = jsonSerializeService;
    }

    public async Task<HttpResponseMessage> PostAsync(string url, object request)
    {
        var json = _jsonSerializeService.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        try
        {
            _logger.LogInformation($"Sending POST request to {url}");
            var response = await _httpClient.PostAsync(url, content);
            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"Error during HTTP request: {ex.Message}");
            throw;
        }
    }
}