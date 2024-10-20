using System.Security.Cryptography.X509Certificates;

namespace BankIdDemoApp.Services;
public interface ICertificateService
{
    X509Certificate2 GetCertificate();
}

public class CertificateService : ICertificateService
{
    private readonly ILogger<CertificateService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _certificatePath;
    private readonly string _certificatePassword;

    public CertificateService(ILogger<CertificateService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        _certificatePath = _configuration["BankID:CertificatePath"]
                           ?? throw new ArgumentNullException("CertificatePath is missing in configuration");
        _certificatePassword = _configuration["BankID:CertificatePassword"]
                               ?? throw new ArgumentNullException("CertificatePassword is missing in configuration");
    }

    public X509Certificate2 GetCertificate()
    {
        try
        {
            _logger.LogInformation("Loading certificate from path");
            var cert = new X509Certificate2(_certificatePath, _certificatePassword);
            _logger.LogInformation("Certificate successfully loaded");
            return cert;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error loading certificate: {ex.Message}");
            throw new InvalidOperationException("Error loading certificate", ex);
        }
    }
}
