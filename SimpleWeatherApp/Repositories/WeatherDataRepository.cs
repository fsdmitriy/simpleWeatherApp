using Microsoft.EntityFrameworkCore;
using SimpleWeatherApp.Data;
using SimpleWeatherApp.Models;

namespace SimpleWeatherApp.Repositories;

public class WeatherDataRepository : IWeatherDataRepository
{
    private readonly WeatherDbContext _dbContext;

    public WeatherDataRepository(WeatherDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WeatherData?> GetByCityNameAsync(string cityName)
    {
        return await _dbContext.WeatherData.FirstOrDefaultAsync(w => w.CityName == cityName);
    }

    public async Task<List<string>> GetAllCityNamesAsync()
    {
        return await _dbContext.WeatherData
            .Select(w => w.CityName)
            .Distinct()
            .ToListAsync();
    }

    public async Task AddAsync(WeatherData data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (string.IsNullOrEmpty(data.CityName)) throw new ArgumentException("CityName cannot be null or empty");
        if (string.IsNullOrEmpty(data.Country)) throw new ArgumentException("Country cannot be null or empty");

        var existingData = await GetByCityNameAsync(data.CityName);
        if (existingData != null)
        {
            throw new InvalidOperationException($"WeatherData for {data.CityName} already exists in the database.");
        }

        _dbContext.WeatherData.Add(data);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(WeatherData data)
    {
        var existingData = await GetByCityNameAsync(data.CityName) 
            ?? throw new ArgumentNullException(nameof(data), $"Cannot find the weather data for '{data.CityName}'");

        existingData.Temperature = data.Temperature;
        existingData.Humidity = data.Humidity;
        existingData.WindSpeed = data.WindSpeed;
        existingData.LastUpdated = DateTime.UtcNow;

        _dbContext.WeatherData.Update(existingData);
        await _dbContext.SaveChangesAsync();
    }
}
