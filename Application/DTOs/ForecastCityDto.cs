using System.Text.Json.Serialization;

namespace WeatherConsoleClient.Application.DTOs;

public class ForecastCityDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }
}
