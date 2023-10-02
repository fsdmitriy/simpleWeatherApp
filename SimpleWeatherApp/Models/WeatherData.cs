namespace SimpleWeatherApp.Models;

public class WeatherData
{
    public int Id { get; set; }
    public string CityName { get; set; } = String.Empty;
    public string Country { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public DateTime LastUpdated { get; set; }
}
