using System.Text.Json.Serialization;

namespace WeatherConsoleClient.Application.DTOs;

public class WindDto
{
    [JsonPropertyName("speed")]
    public decimal Speed { get; set; }

    [JsonPropertyName("deg")]
    public decimal Direction { get; set; }
}
