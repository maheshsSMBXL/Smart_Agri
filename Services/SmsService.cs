using System.Text;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Agri_Smart.Services
{
    public class SmsService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                string apiKey = _configuration["SpringEdge:ApiKey"];
                string senderId = _configuration["SpringEdge:SenderId"];
                string url = "https://instantalerts.co/api/web/send";

                var payload = new
                {
                    apikey = apiKey,
                    sender = senderId,
                    to = phoneNumber,
                    message = message,
                    format = "json"
                };

                string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending SMS: {ex.Message}");
                return false;
            }
        }
    }
}
