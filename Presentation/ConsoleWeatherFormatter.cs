using System.Text;
using WeatherConsoleClient.Application.DTOs;
using WeatherConsoleClient.Application.Interfaces;

namespace WeatherConsoleClient.Presentation;

public class ConsoleWeatherFormatter : IWeatherFormatter
{
    public string FormatCurrentWeather(
        CurrentWeatherDto weather,
        bool useFahrenheit)
    {
        var temperature = ConvertTemperature(
            weather.Main.Temperature,
            useFahrenheit);

        var feelsLike = ConvertTemperature(
            weather.Main.FeelsLike,
            useFahrenheit);

        var temperatureUnit = GetTemperatureUnit(
            useFahrenheit);

        var builder = new StringBuilder();

        builder.AppendLine();
        builder.AppendLine("========================================");
        builder.AppendLine("CURRENT WEATHER");
        builder.AppendLine("========================================");
        builder.AppendLine();
        builder.AppendLine($"City        : {weather.Name}");
        builder.AppendLine(
            $"Temperature : {temperature:F2} {temperatureUnit}");
        builder.AppendLine(
            $"Feels Like  : {feelsLike:F2} {temperatureUnit}");
        builder.AppendLine(
            $"Humidity    : {weather.Main.Humidity} %");
        builder.AppendLine(
            $"Pressure    : {weather.Main.Pressure} hPa");

        if (weather.Weather.Count > 0)
        {
            builder.AppendLine(
                $"Condition   : {weather.Weather[0].Description}");
        }

        builder.AppendLine(
            $"Wind Speed  : {weather.Wind.Speed:F2} m/s");

        AppendHotWeatherAlert(
            builder,
            weather.Main.Temperature);

        return builder.ToString();
    }

    public string FormatForecast(
        ForecastDto forecast,
        IEnumerable<ForecastItemDto> forecastItems,
        bool useFahrenheit)
    {
        var items = forecastItems.ToList();
        var temperatureUnit = GetTemperatureUnit(
            useFahrenheit);

        var builder = new StringBuilder();

        builder.AppendLine();
        builder.AppendLine("========================================");
        builder.AppendLine("5-DAY / 3-HOUR FORECAST");
        builder.AppendLine("========================================");
        builder.AppendLine();
        builder.AppendLine($"City: {forecast.City.Name}");
        builder.AppendLine();

        foreach (var item in items)
        {
            var condition = item.Weather.Count > 0
                ? item.Weather[0].Description
                : "Unknown";

            var precipitation =
                item.ProbabilityOfPrecipitation * 100;

            var temperature = ConvertTemperature(
                item.Main.Temperature,
                useFahrenheit);

            var localDateTime = GetForecastLocalDateTime(
                item,
                forecast.City.Timezone);

            builder.AppendLine(
                $"{localDateTime:yyyy-MM-dd HH:mm:ss} " +
                $"{temperature,7:F1} {temperatureUnit}   " +
                $"{condition,-18}" +
                $"{precipitation,5:F0}%");

            AppendRainAlert(
                builder,
                item.ProbabilityOfPrecipitation);

            AppendHotWeatherAlert(
                builder,
                item.Main.Temperature);
        }

        AppendForecastSummary(
            builder,
            items,
            useFahrenheit);

        return builder.ToString();
    }

    public string FormatDashboard(
        CurrentWeatherDto currentWeather,
        ForecastDto forecast,
        bool useFahrenheit)
    {
        var temperature = ConvertTemperature(
            currentWeather.Main.Temperature,
            useFahrenheit);

        var feelsLike = ConvertTemperature(
            currentWeather.Main.FeelsLike,
            useFahrenheit);

        var temperatureUnit = GetTemperatureUnit(
            useFahrenheit);

        var builder = new StringBuilder();

        builder.AppendLine();
        builder.AppendLine("========================================");
        builder.AppendLine("WEATHER DASHBOARD");
        builder.AppendLine("========================================");
        builder.AppendLine();
        builder.AppendLine($"City: {currentWeather.Name}");
        builder.AppendLine();
        builder.AppendLine("CURRENT WEATHER");
        builder.AppendLine("----------------------------------------");
        builder.AppendLine(
            $"Temperature : {temperature:F1} {temperatureUnit}");
        builder.AppendLine(
            $"Feels Like  : {feelsLike:F1} {temperatureUnit}");
        builder.AppendLine(
            $"Humidity    : {currentWeather.Main.Humidity}%");

        if (currentWeather.Weather.Count > 0)
        {
            builder.AppendLine(
                $"Condition   : {currentWeather.Weather[0].Description}");
        }

        AppendHotWeatherAlert(
            builder,
            currentWeather.Main.Temperature);

        builder.AppendLine();
        builder.AppendLine("FORECAST");
        builder.AppendLine("----------------------------------------");

        foreach (var item in forecast.Items)
        {
            var condition = item.Weather.Count > 0
                ? item.Weather[0].Description
                : "Unknown";

            var precipitation =
                item.ProbabilityOfPrecipitation * 100;

            var forecastTemperature = ConvertTemperature(
                item.Main.Temperature,
                useFahrenheit);

            builder.AppendLine(
                $"{item.DateTimeText,-20}" +
                $"{forecastTemperature,6:F1} {temperatureUnit}   " +
                $"{condition,-18}" +
                $"{precipitation,4:F0}%");

            AppendRainAlert(
                builder,
                item.ProbabilityOfPrecipitation);

            AppendHotWeatherAlert(
                builder,
                item.Main.Temperature);
        }

        AppendForecastSummary(
            builder,
            forecast.Items,
            useFahrenheit);

        return builder.ToString();
    }

    private static void AppendForecastSummary(
        StringBuilder builder,
        IEnumerable<ForecastItemDto> forecastItems,
        bool useFahrenheit)
    {
        var items = forecastItems.ToList();

        if (items.Count == 0)
        {
            return;
        }

        var highestTemperature = items.Max(
            item => item.Main.Temperature);

        var lowestTemperature = items.Min(
            item => item.Main.Temperature);

        var averageTemperature = items.Average(
            item => item.Main.Temperature);

        var highestRainProbability = items.Max(
            item => item.ProbabilityOfPrecipitation) * 100;

        highestTemperature = ConvertTemperature(
            highestTemperature,
            useFahrenheit);

        lowestTemperature = ConvertTemperature(
            lowestTemperature,
            useFahrenheit);

        averageTemperature = ConvertTemperature(
            averageTemperature,
            useFahrenheit);

        var temperatureUnit = GetTemperatureUnit(
            useFahrenheit);

        builder.AppendLine();
        builder.AppendLine("FORECAST SUMMARY");
        builder.AppendLine("----------------------------------------");
        builder.AppendLine(
            $"Highest Temperature : {highestTemperature:F1} {temperatureUnit}");
        builder.AppendLine(
            $"Lowest Temperature  : {lowestTemperature:F1} {temperatureUnit}");
        builder.AppendLine(
            $"Average Temperature : {averageTemperature:F1} {temperatureUnit}");
        builder.AppendLine(
            $"Highest Rain Chance : {highestRainProbability:F0} %");
    }

    private static void AppendRainAlert(
        StringBuilder builder,
        decimal precipitationProbability)
    {
        if (precipitationProbability >= 0.60m)
        {
            builder.AppendLine(
                "RAIN ALERT: High probability of precipitation.");
        }
    }

    private static void AppendHotWeatherAlert(
        StringBuilder builder,
        decimal temperatureCelsius)
    {
        if (temperatureCelsius > 35m)
        {
            builder.AppendLine("WEATHER ALERT:");
            builder.AppendLine(
                "High temperature detected.");
        }
    }

    private static DateTimeOffset GetForecastLocalDateTime(
        ForecastItemDto item,
        int timezone)
    {
        var cityOffset = TimeSpan.FromSeconds(timezone);

        return DateTimeOffset
            .FromUnixTimeSeconds(item.Timestamp)
            .ToOffset(cityOffset);
    }

    private static decimal ConvertTemperature(
        decimal celsius,
        bool useFahrenheit)
    {
        if (!useFahrenheit)
        {
            return celsius;
        }

        return (celsius * 9m / 5m) + 32m;
    }

    private static string GetTemperatureUnit(
        bool useFahrenheit)
    {
        return useFahrenheit ? "°F" : "°C";
    }
}
