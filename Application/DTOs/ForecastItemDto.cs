using System.Text.Json.Serialization;

namespace WeatherConsoleClient.Application.DTOs;

public class ForecastItemDto
{
    [JsonPropertyName("dt")]
    public long Timestamp { get; set; }

    [JsonPropertyName("dt_txt")]
    public string DateTimeText { get; set; } = string.Empty;

    [JsonPropertyName("main")]
    public ForecastMainDto Main { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<WeatherConditionDto> Weather { get; set; } = [];

    [JsonPropertyName("clouds")]
    public ForecastCloudsDto Clouds { get; set; } = new();

    [JsonPropertyName("wind")]
    public WindDto Wind { get; set; } = new();

    [JsonPropertyName("pop")]
    public decimal ProbabilityOfPrecipitation { get; set; }
}
