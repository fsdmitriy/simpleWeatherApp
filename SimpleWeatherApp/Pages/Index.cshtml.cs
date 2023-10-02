using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleWeatherApp.Services;

namespace SimpleWeatherApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly WeatherService _weatherService;

        [BindProperty]
        public string City { get; set; } = string.Empty;

        public IndexModel(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public async Task OnGetAsync(string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                return;
            }

            var weatherData = await _weatherService.GetWeatherDataByCityAsync(city);

            ViewData["CityName"] = weatherData.CityName;
            ViewData["Country"] = weatherData.Country;
            ViewData["Temperature"] = weatherData.Temperature;
            ViewData["Humidity"] = weatherData.Humidity;
            ViewData["WindSpeed"] = weatherData.WindSpeed;
            ViewData["LastUpdated"] = weatherData.LastUpdated.ToLocalTime();
        }
    }
}