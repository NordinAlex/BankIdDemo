using BankIdDemoApp.Services;
using System.Net.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddScoped<IJsonSerializeService, JsonSerializeService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<IQrCodeService, QrCodeService>();
builder.Services.AddScoped<IBankIdService, BankIdService>();
builder.Services.AddScoped<IBankIdAuthService, BankIdAuthService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddHttpClient<IBankIdHttpClient, BankIdHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BankID:BaseUrl"] ?? throw new ArgumentNullException("BankIDs url is missing in configuration"));

})
.ConfigurePrimaryHttpMessageHandler((sp) =>
{
    var handler = new HttpClientHandler();
    var certService = sp.GetRequiredService<ICertificateService>();
    var certificate = certService.GetCertificate();
    handler.ClientCertificates.Add(certificate);


    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, sslPolicyErrors) => true;
    }
    else
    {
        handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, sslPolicyErrors) => sslPolicyErrors == SslPolicyErrors.None;
    }

    return handler;
});


// IHttpContextAccessor
builder.Services.AddHttpContextAccessor();



builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// CORS
app.UseCors(builder =>
{
    builder.AllowAnyOrigin();
    builder.AllowAnyMethod();
    builder.AllowAnyHeader();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
