using System.Text.Json.Serialization;

namespace WeatherConsoleClient.Application.DTOs;

public class WeatherConditionDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("main")]
    public string Main { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
