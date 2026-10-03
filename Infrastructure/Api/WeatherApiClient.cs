using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherConsoleClient.Application.DTOs;
using WeatherConsoleClient.Application.Interfaces;
using WeatherConsoleClient.Configuration;

namespace WeatherConsoleClient.Infrastructure.Api;

public class WeatherApiClient : IWeatherApiClient
{
    private readonly HttpClient _httpClient;
    private readonly OpenWeatherOptions _options;
    private readonly ILogger<WeatherApiClient> _logger;

    public WeatherApiClient(
        HttpClient httpClient,
        IOptions<OpenWeatherOptions> options,
        ILogger<WeatherApiClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<CurrentWeatherDto?> GetCurrentWeatherAsync(
        string city,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Requesting current weather for {City}",
            city);

        var url =
            $"data/2.5/weather" +
            $"?q={Uri.EscapeDataString(city)}" +
            $"&appid={_options.ApiKey}" +
            $"&units={_options.Units}";

        var response = await _httpClient.GetAsync(
            url,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CurrentWeatherDto>(
            cancellationToken);
    }

    public async Task<ForecastDto?> GetForecastAsync(
        string city,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Requesting forecast for {City}",
            city);

        var url =
            $"data/2.5/forecast" +
            $"?q={Uri.EscapeDataString(city)}" +
            $"&appid={_options.ApiKey}" +
            $"&units={_options.Units}";

        var response = await _httpClient.GetAsync(
            url,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ForecastDto>(
            cancellationToken);
    }
}
