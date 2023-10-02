using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SimpleWeatherApp.Data;
using SimpleWeatherApp.Models;
using SimpleWeatherApp.Repositories;
using SimpleWeatherApp.Services;

namespace SimpleWeatherApp.Test;

[TestFixture]
public class WeatherServiceTests
{
    private Mock<IConfiguration> _configurationMock;
    private Mock<IWeatherDataRepository> _weatherDataRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _configurationMock = new Mock<IConfiguration>();
        _weatherDataRepositoryMock = new Mock<IWeatherDataRepository>();

        // Arrange
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.json");
        var configuration = configurationBuilder.Build();
        var expectedApiKey = configuration.GetValue<string>("OpenWeatherMap:ApiKey");
        var expectedApiUrl = configuration.GetValue<string>("OpenWeatherMap:ApiUrl");

        _configurationMock.Setup(x => x.GetSection("OpenWeatherMap:ApiKey").Value).Returns(expectedApiKey);
        _configurationMock.Setup(x => x.GetSection("OpenWeatherMap:ApiUrl").Value).Returns(expectedApiUrl);
    }

    [TestCase("London")]
    [TestCase("New York")]
    [TestCase("Paris")]
    public async Task GetWeatherDataByCityAsync_Returns_Existing_Data(string cityName)
    {
        // Arrange
        var weatherData = new WeatherData
        {
            CityName = cityName,
            Country = "UK",
            Temperature = 15.0f,
            Humidity = 50,
            WindSpeed = 10.0f,
            LastUpdated = DateTime.UtcNow.AddMinutes(-30)
        };

        _weatherDataRepositoryMock.Setup(repo => repo.GetByCityNameAsync(cityName)).ReturnsAsync(weatherData);

        var weatherService = new WeatherService(_configurationMock.Object, _weatherDataRepositoryMock.Object);

        // Act
        var result = await weatherService.GetWeatherDataByCityAsync(cityName);

        // Assert
        Assert.That(result.CityName, Is.EqualTo(cityName));
        _weatherDataRepositoryMock.Verify(repo => repo.GetByCityNameAsync(cityName), Times.Once);
        _weatherDataRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<WeatherData>()), Times.Never);
    }

    [TestCase("London")]
    [TestCase("New York")]
    [TestCase("Paris")]
    public async Task GetWeatherDataByCityAsync_Returns_New_Data(string cityName)
    {
        // Arrange
        var weatherDataFromApi = new WeatherData
        {
            CityName = cityName,
            Country = "UK",
            Temperature = 15.0f,
            Humidity = 50,
            WindSpeed = 10.0f,
            LastUpdated = DateTime.UtcNow
        };

        _weatherDataRepositoryMock.Setup(repo => repo.GetByCityNameAsync(cityName)).ReturnsAsync(weatherDataFromApi);
        _weatherDataRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<WeatherData>())).Returns(Task.CompletedTask);

        var weatherDataRepository = new WeatherDataRepository(new WeatherDbContext(new DbContextOptionsBuilder<WeatherDbContext>()
            .UseInMemoryDatabase(databaseName: "WeatherDatabase")
            .Options));

        var weatherService = new WeatherService(_configurationMock.Object, weatherDataRepository);

        // Act
        var result = await weatherService.GetWeatherDataByCityAsync(cityName);

        // Assert
        Assert.That(result.CityName, Is.EqualTo(cityName));
    }
}
