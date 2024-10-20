namespace BankIdDemoApp.DTOs.BankId.Responses
{
    public class BankIdErrorResponseDto
    {
        public string ErrorCode { get; set; } = null!; // Obligatorisk
        public string Details { get; set; } = null!; // Obligatorisk
    }
}
