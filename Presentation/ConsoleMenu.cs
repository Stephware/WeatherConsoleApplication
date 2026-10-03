using WeatherConsoleClient.Application.DTOs;
using WeatherConsoleClient.Application.Interfaces;

namespace WeatherConsoleClient.Presentation;

public class ConsoleMenu
{
    private readonly IWeatherService _weatherService;
    private readonly IWeatherFormatter _weatherFormatter;

    public ConsoleMenu(
        IWeatherService weatherService,
        IWeatherFormatter weatherFormatter)
    {
        _weatherService = weatherService;
        _weatherFormatter = weatherFormatter;
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

        Console.Write(
            _weatherFormatter.FormatCurrentWeather(
                weather,
                useFahrenheit));
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

        var forecastItems = FilterForecastItems(
            forecast,
            forecastFilter).ToList();

        if (forecastItems.Count == 0)
        {
            Console.WriteLine(
                "No forecast entries found for the selected day.");
            return;
        }

        Console.Write(
            _weatherFormatter.FormatForecast(
                forecast,
                forecastItems,
                useFahrenheit));
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

        var currentWeatherTask =
            _weatherService.GetCurrentWeatherAsync(
                city,
                cancellationToken);

        var forecastTask =
            _weatherService.GetForecastAsync(
                city,
                cancellationToken);

        await Task.WhenAll(
            currentWeatherTask,
            forecastTask);

        var currentWeather = await currentWeatherTask;
        var forecast = await forecastTask;

        if (currentWeather is null || forecast is null)
        {
            Console.WriteLine(
                "Unable to retrieve weather information.");
            return;
        }

        Console.Write(
            _weatherFormatter.FormatDashboard(
                currentWeather,
                forecast,
                useFahrenheit));
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
}
