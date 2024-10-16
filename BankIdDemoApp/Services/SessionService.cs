using BankIdDemoApp.Models.BankId;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BankIdDemoApp.Services;

public interface ISessionService
{
    void StoreQrResponse(BankIdAuthQrResponse qrResponse);
    BankIdAuthQrResponse GetQrResponse();
    void StoreOrderRef(string orderRef);
    string GetOrderRef();
    void StoreCompletionData(string data);
    string GetCompletionData();
}

public class SessionService : ISessionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJsonSerializeService _jsonSerializeService;

    public SessionService(IHttpContextAccessor httpContextAccessor, IJsonSerializeService jsonSerializeService)
    {
        _httpContextAccessor = httpContextAccessor;
        _jsonSerializeService = jsonSerializeService;
    }

    public void StoreQrResponse(BankIdAuthQrResponse qrResponse)
    {
        //var json = JsonSerializer.Serialize(qrResponse);
        var json = _jsonSerializeService.Serialize(qrResponse);
        _httpContextAccessor.HttpContext.Session.SetString("BankIdAuthQrResponse", json);
    }

    public BankIdAuthQrResponse GetQrResponse()
    {
        var json = _httpContextAccessor.HttpContext.Session.GetString("BankIdAuthQrResponse");
        return _jsonSerializeService.Deserialize<BankIdAuthQrResponse>(json);
    }

    public void StoreCompletionData(string data)
    {
        _httpContextAccessor.HttpContext.Session.SetString("CompletionData", data);
    }

    public string GetCompletionData()
    {
        return _httpContextAccessor.HttpContext.Session.GetString("CompletionData");
    }

    public void StoreOrderRef(string orderRef)
    {
        _httpContextAccessor.HttpContext.Session.SetString("OrderRef", orderRef);
    }

    public string GetOrderRef()
    {
        return _httpContextAccessor.HttpContext.Session.GetString("OrderRef");
    }
}