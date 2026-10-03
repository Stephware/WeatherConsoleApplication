using WeatherConsoleClient.Application.DTOs;
using WeatherConsoleClient.Application.Interfaces;

namespace WeatherConsoleClient.Presentation;

public class ConsoleMenu
{
    private readonly IWeatherService _weatherService;

    public ConsoleMenu(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            DisplayMenu();

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ShowCurrentWeatherAsync(cancellationToken);
                    break;

                case "2":
                    await ShowForecastAsync(cancellationToken);
                    break;

                case "3":
                    await ShowDashboardAsync(cancellationToken);
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }

    private static void DisplayMenu()
    {
        Console.Clear();

        Console.WriteLine("========================================");
        Console.WriteLine("       WEATHER CONSOLE CLIENT");
        Console.WriteLine("========================================");
        Console.WriteLine();
        Console.WriteLine("1. Current Weather");
        Console.WriteLine("2. 5-Day / 3-Hour Forecast");
        Console.WriteLine("3. Weather Dashboard");
        Console.WriteLine("0. Exit");
        Console.WriteLine();
        Console.Write("Enter your choice: ");
    }

    private async Task ShowCurrentWeatherAsync(
        CancellationToken cancellationToken)
    {
        Console.WriteLine();
        Console.Write("Enter city: ");

        var city = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(city))
        {
            Console.WriteLine("City is required.");
            return;
        }

        var weather = await _weatherService.GetCurrentWeatherAsync(
            city,
            cancellationToken);

        if (weather is null)
        {
            Console.WriteLine("City not found.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("CURRENT WEATHER");
        Console.WriteLine("========================================");
        Console.WriteLine();
        Console.WriteLine($"City        : {weather.Name}");
        Console.WriteLine($"Temperature : {weather.Main.Temperature:F2} °C");
        Console.WriteLine($"Feels Like  : {weather.Main.FeelsLike:F2} °C");
        Console.WriteLine($"Humidity    : {weather.Main.Humidity} %");
        Console.WriteLine($"Pressure    : {weather.Main.Pressure} hPa");

        if (weather.Weather.Count > 0)
        {
            Console.WriteLine($"Condition   : {weather.Weather[0].Description}");
        }

        Console.WriteLine($"Wind Speed  : {weather.Wind.Speed:F2} m/s");
    }

    private async Task ShowForecastAsync(
        CancellationToken cancellationToken)
    {
        Console.WriteLine();
        Console.Write("Enter city: ");

        var city = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(city))
        {
            Console.WriteLine("City is required.");
            return;
        }

        var forecast = await _weatherService.GetForecastAsync(
            city,
            cancellationToken);

        if (forecast is null)
        {
            Console.WriteLine("City not found.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("5-DAY / 3-HOUR FORECAST");
        Console.WriteLine("========================================");
        Console.WriteLine();
        Console.WriteLine($"City: {forecast.City.Name}");
        Console.WriteLine();

        foreach (var item in forecast.Items)
        {
            var condition = item.Weather.Count > 0
                ? item.Weather[0].Description
                : "Unknown";

            var precipitation = item.ProbabilityOfPrecipitation * 100;

            Console.WriteLine(
                $"{item.DateTimeText,-20}" +
                $"{item.Main.Temperature,7:F1} °C   " +
                $"{condition,-18}" +
                $"{precipitation,5:F0}%");
        }
    }

    private async Task ShowDashboardAsync(
        CancellationToken cancellationToken)
    {
        Console.WriteLine();
        Console.Write("Enter city: ");

        var city = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(city))
        {
            Console.WriteLine("City is required.");
            return;
        }

        var currentWeatherTask = _weatherService.GetCurrentWeatherAsync(
            city,
            cancellationToken);

        var forecastTask = _weatherService.GetForecastAsync(
            city,
            cancellationToken);

        await Task.WhenAll(
            currentWeatherTask,
            forecastTask);

        var currentWeather = await currentWeatherTask;
        var forecast = await forecastTask;

        if (currentWeather is null || forecast is null)
        {
            Console.WriteLine("Unable to retrieve weather information.");
            return;
        }

        DisplayDashboard(
            currentWeather,
            forecast);
    }

    private static void DisplayDashboard(
        CurrentWeatherDto currentWeather,
        ForecastDto forecast)
    {
        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("WEATHER DASHBOARD");
        Console.WriteLine("========================================");
        Console.WriteLine();
        Console.WriteLine($"City: {currentWeather.Name}");
        Console.WriteLine();
        Console.WriteLine("CURRENT WEATHER");
        Console.WriteLine("----------------------------------------");
        Console.WriteLine(
            $"Temperature : {currentWeather.Main.Temperature:F1} °C");
        Console.WriteLine(
            $"Feels Like  : {currentWeather.Main.FeelsLike:F1} °C");
        Console.WriteLine(
            $"Humidity    : {currentWeather.Main.Humidity}%");

        if (currentWeather.Weather.Count > 0)
        {
            Console.WriteLine(
                $"Condition   : {currentWeather.Weather[0].Description}");
        }

        Console.WriteLine();
        Console.WriteLine("FORECAST");
        Console.WriteLine("----------------------------------------");

        foreach (var item in forecast.Items)
        {
            var condition = item.Weather.Count > 0
                ? item.Weather[0].Description
                : "Unknown";

            var precipitation = item.ProbabilityOfPrecipitation * 100;

            Console.WriteLine(
                $"{item.DateTimeText,-20}" +
                $"{item.Main.Temperature,6:F1} °C   " +
                $"{condition,-18}" +
                $"{precipitation,4:F0}%");
        }
    }
}
