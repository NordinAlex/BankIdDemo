namespace BankIdDemoApp.ViewModels
{
    public class BankIdAuthViewModel
    {
        public string? AutoStartToken { get; set; }
        public string? QrCodeBase64 { get; set; }
        public string? LaunchingType { get; set; }
        public string? ReturnUrl { get; set; }        
    }
}
