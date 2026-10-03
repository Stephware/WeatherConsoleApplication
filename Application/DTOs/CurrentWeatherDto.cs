using System.Text.Json.Serialization;

namespace WeatherConsoleClient.Application.DTOs;

public class CurrentWeatherDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("main")]
    public CurrentWeatherMainDto Main { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<WeatherConditionDto> Weather { get; set; } = [];

    [JsonPropertyName("wind")]
    public WindDto Wind { get; set; } = new();
}
