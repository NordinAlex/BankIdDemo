using System.Text.Json;
using System.Text.Json.Serialization;

namespace BankIdDemoApp.Services
{
    public interface IJsonSerializeService
    {
        T Deserialize<T>(string content);
        Task<T> DeserializeAsync<T>(Stream contentStream, CancellationToken cancellationToken = default);
        string Serialize(object obj, JsonSerializerOptions options = null!);
    }

    public class JsonSerializeService : IJsonSerializeService
    {

        public string Serialize(object obj, JsonSerializerOptions options = null!)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            options ??= new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() } // Använd inbyggd JsonStringEnumConverter
            };

            return JsonSerializer.Serialize(obj, options);
        }

        public T Deserialize<T>(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException(nameof(content));
            }

            try
            {
                // Standard JSON deserialisering
                return JsonSerializer.Deserialize<T>(content);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Error deserializing JSON content", ex);
            }
        }

        public async Task<T> DeserializeAsync<T>(Stream contentStream, CancellationToken cancellationToken = default)
        {
            if (contentStream == null)
            {
                throw new ArgumentNullException(nameof(contentStream));
            }

            try
            {
                return await JsonSerializer.DeserializeAsync<T>(contentStream, cancellationToken: cancellationToken);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Error deserializing JSON stream", ex);
            }
        }
    }
}
