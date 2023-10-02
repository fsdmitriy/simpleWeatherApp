using Newtonsoft.Json;
using RestSharp;
using SimpleWeatherApp.Models;
using SimpleWeatherApp.Repositories;

namespace SimpleWeatherApp.Services;

public class WeatherService
{
    private readonly IConfiguration _config;
    private readonly IWeatherDataRepository _weatherDataRepository;

    public WeatherService(IConfiguration config, IWeatherDataRepository weatherDataRepository)
    {
        _config = config;
        _weatherDataRepository = weatherDataRepository;
    }

    public async Task<WeatherData> GetWeatherDataByCityAsync(string cityName)
    {
        var existingData = await _weatherDataRepository.GetByCityNameAsync(cityName);
        if (existingData is not null)
        {
            return existingData;
        }

        var weatherData = await GetWeatherDataFromApiAsync(cityName);

        await AddWeatherDataAsync(weatherData);

        return weatherData;
    }

    private Task AddWeatherDataAsync(WeatherData weatherData)
        => _weatherDataRepository.AddAsync(weatherData);

    public async Task<WeatherData> GetWeatherDataFromApiAsync(string cityName)
    {
        var apiUrl = _config.GetValue<string>("OpenWeatherMap:ApiUrl") ?? throw new NullReferenceException("ApiUrl parameter cannot be null");
        var client = new RestClient(apiUrl);
        var request = new RestRequest("/data/2.5/weather", Method.Get);
        request.AddParameter("q", cityName);
        request.AddParameter("appid", _config.GetValue<string>("OpenWeatherMap:ApiKey"));
        request.AddParameter("units", "metric");

        var response = await client.ExecuteAsync(request);
        var data = JsonConvert.DeserializeObject<dynamic>(response.Content ?? throw new NullReferenceException(nameof(response.Content)))
            ?? throw new NullReferenceException("Cannot handle null data");

        return new WeatherData
        {
            CityName = data.name,
            Country = data.sys.country,
            Temperature = data.main.temp,
            Humidity = data.main.humidity,
            WindSpeed = data.wind.speed,
            LastUpdated = DateTime.UtcNow
        };
    }
}
