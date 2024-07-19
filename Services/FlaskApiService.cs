using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.IO;

namespace Agri_Smart.Services
{
    public class FlaskApiService
    {
        private readonly HttpClient _httpClient;

        public FlaskApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> PredictLeafDiseaseAsync(string imagePath)
        {
            using (var form = new MultipartFormDataContent())
            {
                using (var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    using (var content = new StreamContent(fileStream))
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                        form.Add(content, "image", Path.GetFileName(imagePath));
                        var response = await _httpClient.PostAsync("http://192.168.1.174:5001/predict", form);
                        response.EnsureSuccessStatusCode();
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }
    }
}
