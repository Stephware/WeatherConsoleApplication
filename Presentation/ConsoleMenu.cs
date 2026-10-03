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

        var useFahrenheit = SelectTemperatureUnit();

        var weather = await _weatherService.GetCurrentWeatherAsync(
            city,
            cancellationToken);

        if (weather is null)
        {
            Console.WriteLine("City not found.");
            return;
        }

        var temperature = ConvertTemperature(
            weather.Main.Temperature,
            useFahrenheit);

        var feelsLike = ConvertTemperature(
            weather.Main.FeelsLike,
            useFahrenheit);

        var temperatureUnit = GetTemperatureUnit(useFahrenheit);

        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("CURRENT WEATHER");
        Console.WriteLine("========================================");
        Console.WriteLine();
        Console.WriteLine($"City        : {weather.Name}");
        Console.WriteLine(
            $"Temperature : {temperature:F2} {temperatureUnit}");
        Console.WriteLine(
            $"Feels Like  : {feelsLike:F2} {temperatureUnit}");
        Console.WriteLine($"Humidity    : {weather.Main.Humidity} %");
        Console.WriteLine($"Pressure    : {weather.Main.Pressure} hPa");

        if (weather.Weather.Count > 0)
        {
            Console.WriteLine(
                $"Condition   : {weather.Weather[0].Description}");
        }

        Console.WriteLine($"Wind Speed  : {weather.Wind.Speed:F2} m/s");

        DisplayHotWeatherAlert(
            weather.Main.Temperature);
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

        var useFahrenheit = SelectTemperatureUnit();
        var forecastFilter = SelectForecastFilter();

        var forecast = await _weatherService.GetForecastAsync(
            city,
            cancellationToken);

        if (forecast is null)
        {
            Console.WriteLine("City not found.");
            return;
        }

        var temperatureUnit = GetTemperatureUnit(useFahrenheit);

        var forecastItems = FilterForecastItems(
            forecast,
            forecastFilter).ToList();

        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("5-DAY / 3-HOUR FORECAST");
        Console.WriteLine("========================================");
        Console.WriteLine();
        Console.WriteLine($"City: {forecast.City.Name}");
        Console.WriteLine();

        if (forecastItems.Count == 0)
        {
            Console.WriteLine(
                "No forecast entries found for the selected day.");
            return;
        }

        foreach (var item in forecastItems)
        {
            var condition = item.Weather.Count > 0
                ? item.Weather[0].Description
                : "Unknown";

            var precipitation = item.ProbabilityOfPrecipitation * 100;

            var temperature = ConvertTemperature(
                item.Main.Temperature,
                useFahrenheit);

            var localDateTime = GetForecastLocalDateTime(
                item,
                forecast.City.Timezone);

            Console.WriteLine(
                $"{localDateTime:yyyy-MM-dd HH:mm:ss} " +
                $"{temperature,7:F1} {temperatureUnit}   " +
                $"{condition,-18}" +
                $"{precipitation,5:F0}%");

            DisplayRainAlert(
                item.ProbabilityOfPrecipitation);

            DisplayHotWeatherAlert(
                item.Main.Temperature);
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

        var useFahrenheit = SelectTemperatureUnit();

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
            forecast,
            useFahrenheit);
    }

    private static void DisplayDashboard(
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

        var temperatureUnit = GetTemperatureUnit(useFahrenheit);

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
            $"Temperature : {temperature:F1} {temperatureUnit}");
        Console.WriteLine(
            $"Feels Like  : {feelsLike:F1} {temperatureUnit}");
        Console.WriteLine(
            $"Humidity    : {currentWeather.Main.Humidity}%");

        if (currentWeather.Weather.Count > 0)
        {
            Console.WriteLine(
                $"Condition   : {currentWeather.Weather[0].Description}");
        }

        DisplayHotWeatherAlert(
            currentWeather.Main.Temperature);

        Console.WriteLine();
        Console.WriteLine("FORECAST");
        Console.WriteLine("----------------------------------------");

        foreach (var item in forecast.Items)
        {
            var condition = item.Weather.Count > 0
                ? item.Weather[0].Description
                : "Unknown";

            var precipitation = item.ProbabilityOfPrecipitation * 100;

            var forecastTemperature = ConvertTemperature(
                item.Main.Temperature,
                useFahrenheit);

            Console.WriteLine(
                $"{item.DateTimeText,-20}" +
                $"{forecastTemperature,6:F1} {temperatureUnit}   " +
                $"{condition,-18}" +
                $"{precipitation,4:F0}%");

            DisplayRainAlert(
                item.ProbabilityOfPrecipitation);

            DisplayHotWeatherAlert(
                item.Main.Temperature);
        }
    }

    private static bool SelectTemperatureUnit()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Select temperature unit:");
            Console.WriteLine("1. Celsius");
            Console.WriteLine("2. Fahrenheit");
            Console.Write("Enter choice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    return false;

                case "2":
                    return true;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private static int SelectForecastFilter()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Select forecast filter:");
            Console.WriteLine("1. Show all forecast entries");
            Console.WriteLine("2. Show today's forecast");
            Console.WriteLine("3. Show tomorrow's forecast");
            Console.Write("Enter choice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    return 1;

                case "2":
                    return 2;

                case "3":
                    return 3;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private static IEnumerable<ForecastItemDto> FilterForecastItems(
        ForecastDto forecast,
        int forecastFilter)
    {
        if (forecastFilter == 1)
        {
            return forecast.Items;
        }

        var cityOffset = TimeSpan.FromSeconds(
            forecast.City.Timezone);

        var targetDate = DateTimeOffset.UtcNow
            .ToOffset(cityOffset)
            .Date;

        if (forecastFilter == 3)
        {
            targetDate = targetDate.AddDays(1);
        }

        return forecast.Items.Where(item =>
            GetForecastLocalDateTime(
                item,
                forecast.City.Timezone).Date == targetDate);
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

    private static void DisplayRainAlert(
        decimal precipitationProbability)
    {
        if (precipitationProbability >= 0.60m)
        {
            Console.WriteLine(
                "RAIN ALERT: High probability of precipitation.");
        }
    }

    private static void DisplayHotWeatherAlert(
        decimal temperatureCelsius)
    {
        if (temperatureCelsius > 35m)
        {
            Console.WriteLine("WEATHER ALERT:");
            Console.WriteLine("High temperature detected.");
        }
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

    private static string GetTemperatureUnit(bool useFahrenheit)
    {
        return useFahrenheit ? "°F" : "°C";
    }
}
