namespace WeatherConsoleClient.Configuration;

public class OpenWeatherOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Units { get; set; } = "metric";
}
