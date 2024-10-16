using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BankIdDemoApp.DTOs.BankId.Requests
{
    public class BankIdAuthRequestDto
    {
        [Required]
        [IpAddress] // Du kan implementera en anpassad validering för IP-adresser
        public string EndUserIp { get; set; } = null!; // Obligatorisk

        public bool? ReturnRisk { get; set; } // Valfri, kan vara true för att returnera riskindikator

        //public string ReturnUrl { get; set; } = string.Empty; // Valfri

        [Base64Encoded]
        public string? UserVisibleData { get; set; } // Valfri, base64-kodad text

        //[Base64Encoded]
        //public string? UserNonVisibleData { get; set; } // Valfri, base64-kodad text

        //public string? UserVisibleDataFormat { get; set; } // Valfri, kan användas för formatering

        //public AppDataDto? App { get; set; } // Valfri, app data (kan vara null om webben används)

        //public WebDataDto? Web { get; set; } // Valfri, web data (kan vara null om appen används)

        public RequirementDto? Requirement { get; set; } // Valfri, krav (kan vara null om inga specifika krav sätts)
    }

    public class AppDataDto
    {
        public string? AppIdentifier { get; set; } // Appens identifierare (t.ex. Android package name eller iOS bundle identifier)
        public string? DeviceOS { get; set; } // Operativsystemversion
        public string? DeviceModelName { get; set; } // Modellnamn (t.ex. iPhone 14)
        public string? DeviceIdentifier { get; set; } // Enhetens identifierare
    }

    public class WebDataDto
    {
        public string? ReferringDomain { get; set; } // Domänen som startar BankID-processen
        public string? UserAgent { get; set; } // User agent (kan användas för att identifiera browser)
        public string? DeviceIdentifier { get; set; } // Identifierare för webbläsaren (t.ex. baserat på cookies)
    }

    public class RequirementDto
    {
        public RiskLevel? Risk { get; set; } // Risknivå (enum istället för sträng)
        //public string? PersonalNumber { get; set; } // Personnummer (valfritt, om det behövs för autentisering)
        //public bool? PinCode { get; set; } // Om PIN-kod krävs
        //public bool? Mrtd { get; set; } // Om nationellt ID-kort eller pass krävs
        //public string[]? CertificatePolicies { get; set; } // Certifikatpolicyn (OID)
        //public string? CardReader { get; set; } // Om kortläsare behövs (kan vara class1 eller class2)
    }

    // Enum för Risknivå
    public enum RiskLevel
    {
        [Description("low")]
        Low,
        [Description("moderate")]
        Moderate
    }

    // Anpassad validering för IP-adress (kan implementeras som en egen attributklass)
    public class IpAddressAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;

            string ipAddress = value.ToString()!;
            return System.Net.IPAddress.TryParse(ipAddress, out _);
        }
    }

    // Anpassad base64-kodning (kan användas som ett attribut för att verifiera base64-format)
    public class Base64EncodedAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;

            string data = value.ToString()!;
            return IsBase64String(data);
        }

        private static bool IsBase64String(string s)
        {
            Span<byte> buffer = new Span<byte>(new byte[s.Length]);
            return Convert.TryFromBase64String(s, buffer, out _);
        }
    }
}
