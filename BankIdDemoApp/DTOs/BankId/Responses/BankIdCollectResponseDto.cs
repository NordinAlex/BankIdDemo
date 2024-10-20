namespace BankIdDemoApp.DTOs.BankId.Responses
{
    public class BankIdCollectResponseDto
    {
        public string OrderRef { get; set; } = null!; // Obligatorisk
        public string Status { get; set; } = null!; // Obligatorisk
        public string? HintCode { get; set; } // Valfri
        public CompletionDataDto? CompletionData { get; set; } // Valfri
    }

    public class CompletionDataDto
    {
        public UserDataDto? User { get; set; } // Användardata
        public DeviceDataDto? Device { get; set; } // Enhetsdata
        public string? BankIdIssueDate { get; set; } // Valfri
        public string? Signature { get; set; } // Valfri
        public string? OcspResponse { get; set; } // Valfri
    }

    public class UserDataDto
    {
        public string? PersonalNumber { get; set; }
        public string? Name { get; set; }
        public string? GivenName { get; set; }
        public string? Surname { get; set; }
    }

    public class DeviceDataDto
    {
        public string? IpAddress { get; set; }
        public string? Uhi { get; set; }
    }
}
