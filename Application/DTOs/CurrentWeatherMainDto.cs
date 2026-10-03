using System.Text.Json.Serialization;

namespace WeatherConsoleClient.Application.DTOs;

public class CurrentWeatherMainDto
{
    [JsonPropertyName("temp")]
    public decimal Temperature { get; set; }

    [JsonPropertyName("feels_like")]
    public decimal FeelsLike { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }
}
