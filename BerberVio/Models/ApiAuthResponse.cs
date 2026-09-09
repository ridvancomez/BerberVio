using System.Text.Json.Serialization;

namespace BerberVio.Models;

public class ApiAuthResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;

    [JsonPropertyName("expiresAt")]
    public DateTime ExpiresAt { get; set; }
}
