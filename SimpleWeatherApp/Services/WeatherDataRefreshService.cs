using SimpleWeatherApp.Repositories;

namespace SimpleWeatherApp.Services;

public class WeatherDataRefreshService : IHostedService, IDisposable
{
    private readonly ILogger<WeatherDataRefreshService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly int _refreshInterval;
    private Timer? _timer;

    public WeatherDataRefreshService(
        ILogger<WeatherDataRefreshService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _refreshInterval = configuration.GetValue<int>("WeatherDataRefreshInterval");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"WeatherDataRefreshService is starting, interval: {_refreshInterval} seconds");

        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_refreshInterval));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("WeatherDataRefreshService is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        _logger.LogInformation("WeatherDataRefreshService is refreshing weather data from API.");

        using var scope = _serviceProvider.CreateScope();

        var weatherService = scope.ServiceProvider.GetRequiredService<WeatherService>();
        var weatherDataRepo = scope.ServiceProvider.GetRequiredService<IWeatherDataRepository>();

        var cities =  await weatherDataRepo.GetAllCityNamesAsync();
        if(cities is null || !cities.Any())
        {
            return;
        }

        foreach (var city in cities)
        {
            try
            {
                var weatherData = await weatherService.GetWeatherDataFromApiAsync(city);
                await weatherDataRepo.UpdateAsync(weatherData);
                _logger.LogInformation($"Weather data for {city} refreshed.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error refreshing weather data for {city}: {ex.Message}");
            }
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
