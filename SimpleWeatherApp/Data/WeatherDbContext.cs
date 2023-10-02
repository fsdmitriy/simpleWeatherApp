using Microsoft.EntityFrameworkCore;
using SimpleWeatherApp.Models;

namespace SimpleWeatherApp.Data;

public class WeatherDbContext: DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options)
    {
    }

    public DbSet<WeatherData> WeatherData { get; set; }
}
