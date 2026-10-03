using WeatherConsoleClient.Application.DTOs;

namespace WeatherConsoleClient.Application.Interfaces;

public interface IWeatherFormatter
{
    string FormatCurrentWeather(
        CurrentWeatherDto weather,
        bool useFahrenheit);

    string FormatForecast(
        ForecastDto forecast,
        IEnumerable<ForecastItemDto> forecastItems,
        bool useFahrenheit);

    string FormatDashboard(
        CurrentWeatherDto currentWeather,
        ForecastDto forecast,
        bool useFahrenheit);
}
