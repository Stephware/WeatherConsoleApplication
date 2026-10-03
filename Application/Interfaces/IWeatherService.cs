using WeatherConsoleClient.Application.DTOs;

namespace WeatherConsoleClient.Application.Interfaces;

public interface IWeatherService
{
    Task<CurrentWeatherDto?> GetCurrentWeatherAsync(
        string city,
        CancellationToken cancellationToken);

    Task<ForecastDto?> GetForecastAsync(
        string city,
        CancellationToken cancellationToken);
}
