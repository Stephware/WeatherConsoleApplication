using System.Text.Json.Serialization;

namespace WeatherConsoleClient.Application.DTOs;

public class ForecastDto
{
    [JsonPropertyName("cnt")]
    public int Count { get; set; }

    [JsonPropertyName("list")]
    public List<ForecastItemDto> Items { get; set; } = [];

    [JsonPropertyName("city")]
    public ForecastCityDto City { get; set; } = new();
}
