using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Agri_Smart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public WeatherForecastController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("forecast")]
        public async Task<IActionResult> GetWeatherForecast([FromQuery] double lat, [FromQuery] double lon)
        {
            // Get the API key and base URL from appsettings.json
            var apiKey = _configuration["OpenWeatherMap:ApiKey"];
            var baseUrl = _configuration["OpenWeatherMap:BaseUrl"];

            // Construct the full API URL
            var url = $"{baseUrl}?lat={lat}&lon={lon}&appid={apiKey}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON content to a dynamic object
                var jsonResponse = JsonSerializer.Deserialize<dynamic>(content);

                // Return the JSON response as an object
                return Ok(jsonResponse);
            }
            else
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }
    }
}
