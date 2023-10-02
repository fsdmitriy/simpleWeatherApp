using Microsoft.EntityFrameworkCore;
using SimpleWeatherApp.Data;
using SimpleWeatherApp.Models;
using SimpleWeatherApp.Repositories;

namespace SimpleWeatherApp.Test;

[TestFixture]
public class WeatherDataRepositoryTests
{
    private WeatherDbContext _dbContext;

    [SetUp]
    public void Setup()
    {
        _dbContext = new WeatherDbContext(new DbContextOptionsBuilder<WeatherDbContext>()
            .UseInMemoryDatabase(databaseName: "WeatherDatabase")
            .Options);
    }

    [Test]
    public async Task AddAsync_Adds_Data_To_Db()
    {
        // Arrange
        var repository = new WeatherDataRepository(_dbContext);
        var weatherData = new WeatherData
        {
            CityName = "New York",
            Country = "US",
            Temperature = 15.0f,
            Humidity = 50,
            WindSpeed = 10.0f,
            LastUpdated = DateTime.UtcNow
        };

        // Act
        await repository.AddAsync(weatherData);

        // Assert
        var result = await _dbContext.WeatherData.FindAsync(weatherData.Id);
        Assert.That(result, Is.EqualTo(weatherData));
    }

    [Test]
    public async Task GetByCityNameAsync_Returns_Null_If_Data_Not_Found()
    {
        // Arrange
        var repository = new WeatherDataRepository(_dbContext);

        // Act
        var result = await repository.GetByCityNameAsync("Kyiv");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByCityNameAsync_Returns_Data_If_Found()
    {
        // Arrange
        var repository = new WeatherDataRepository(_dbContext);
        var weatherData = new WeatherData
        {
            CityName = "London",
            Country = "UK",
            Temperature = 15.0f,
            Humidity = 50,
            WindSpeed = 10.0f,
            LastUpdated = DateTime.UtcNow
        };
        await repository.AddAsync(weatherData);

        // Act
        var result = await repository.GetByCityNameAsync("London");

        // Assert
        Assert.That(result, Is.EqualTo(weatherData));
    }
}
