using SimpleWeatherApp.Models;

namespace SimpleWeatherApp.Repositories;

public interface IWeatherDataRepository
{
    Task<WeatherData?> GetByCityNameAsync(string cityName);
    Task<List<string>> GetAllCityNamesAsync();
    Task AddAsync(WeatherData data);
    Task UpdateAsync(WeatherData data);
}
