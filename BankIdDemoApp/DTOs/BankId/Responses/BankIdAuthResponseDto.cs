namespace BankIdDemoApp.DTOs.BankId.Responses
{
    public class BankIdAuthResponseDto
    {
        public string OrderRef { get; set; } = null!; // Obligatorisk
        public string AutoStartToken { get; set; } = null!; // Obligatorisk
        public string QrStartToken { get; set; } = null!; // Obligatorisk
        public string QrStartSecret { get; set; } = null!; // Obligatorisk
    }
}
