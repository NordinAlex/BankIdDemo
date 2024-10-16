namespace BankIdDemoApp.Models.BankId
{
    public class BankIdAuthQrResponse
    {
        public string qrStartToken { get; set; }
        public string qrStartSecret { get; set; }
        public DateTime orderTime { get; set; }
    }
}
