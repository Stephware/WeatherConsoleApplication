using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WeatherConsoleClient.Application.Interfaces;
using WeatherConsoleClient.Application.Services;
using WeatherConsoleClient.Configuration;
using WeatherConsoleClient.Infrastructure.Api;
using WeatherConsoleClient.Presentation;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: false)
            .Build();

        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
        });

        services.Configure<OpenWeatherOptions>(
            configuration.GetSection("OpenWeather"));

        services.AddHttpClient<IWeatherApiClient, WeatherApiClient>(
            client =>
            {
                client.BaseAddress = new Uri(
                    configuration["OpenWeather:BaseUrl"]!);
            })
            .AddStandardResilienceHandler();

        services.AddScoped<IWeatherService, WeatherService>();
        services.AddScoped<ConsoleMenu>();

        using var serviceProvider = services.BuildServiceProvider();
        using var cancellationTokenSource = new CancellationTokenSource();

        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellationTokenSource.Cancel();

            Console.WriteLine();
            Console.WriteLine("Shutdown requested...");
        };

        var logger = serviceProvider
            .GetRequiredService<ILogger<Program>>();

        try
        {
            var menu = serviceProvider
                .GetRequiredService<ConsoleMenu>();

            await menu.RunAsync(
                cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation(
                "Application was cancelled.");
        }
        catch (Exception ex)
        {
            logger.LogCritical(
                ex,
                "Unhandled application error.");

            Console.WriteLine();
            Console.WriteLine(
                "An unexpected error occurred.");
        }
    }
}
