using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using SixLabors.ImageSharp.PixelFormats;
using Trees_RaysApi.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Agri_Smart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictionController : ControllerBase
    {
        private readonly InferenceSession _session;
        private readonly InferenceSession _leafSessioin;
        private readonly ILogger<PredictionController> _logger;

        public PredictionController(ILogger<PredictionController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            var modelPath = Path.Combine(env.ContentRootPath, "ML_Models", "coffee_leaf_model.onnx");
            _session = new InferenceSession(modelPath);
            var LeafDetectionPath = Path.Combine(env.ContentRootPath, "ML_Models", "leaf_classification_model.onnx");
            _leafSessioin = new InferenceSession(LeafDetectionPath);
        }

        [HttpPost("LeafDetection")]
        public IActionResult Predict(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var inputData = PreprocessImage(file, 224, 224);
            var tensor = new DenseTensor<float>(inputData, new[] { 1, 224, 224, 3 });

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", tensor)
            };

            using var results = _leafSessioin.Run(inputs);
            var output = results.First().AsEnumerable<float>().ToArray();
            var prediction = output[0];

            var isLeaf = prediction > 0.5;
            var result = isLeaf
                ? new { status = true, message = "The image is classified as a leaf." }
                : new { status = false, message = "The image is classified as not a leaf." };
            return Ok(result);
        }

        private float[] PreprocessImage(IFormFile file, int targetWidth, int targetHeight)
        {
            using var stream = file.OpenReadStream();
            using var image = Image.Load<Rgb24>(stream);

            image.Mutate(x => x.Resize(targetWidth, targetHeight));

            var imageData = new float[targetWidth * targetHeight * 3];
            for (int y = 0; y < targetHeight; y++)
            {
                for (int x = 0; x < targetWidth; x++)
                {
                    var pixel = image[x, y];
                    var index = (y * targetWidth + x) * 3;
                    imageData[index] = pixel.R / 255f;
                    imageData[index + 1] = pixel.G / 255f;
                    imageData[index + 2] = pixel.B / 255f;
                }
            }

            return imageData;
        }

        [HttpPost("predict")]
        public async Task<IActionResult> Predict([FromForm] PredictRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded" });
            }

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, request.File.FileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                var predictedClass = PredictDisease(filePath);

                System.IO.File.Delete(filePath);

                return Ok(new
                {
                    predicted_class = predictedClass
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error predicting disease");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        private string PredictDisease(string imagePath)
        {
            var imageTensor = PreprocessImage(imagePath);

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", imageTensor)
            };

            using var results = _session.Run(inputs);
            var output = results.First().AsEnumerable<float>().ToArray();
            int predictedClassIndex = Array.IndexOf(output, output.Max());
            string[] diseaseClasses = { "healthy", "miner", "rust", "phoma", "cercospora" };

            return diseaseClasses[predictedClassIndex];
        }

        private Tensor<float> PreprocessImage(string imagePath)
        {
            using var image = Image.Load<Rgb24>(imagePath);
            image.Mutate(x => x.Resize(224, 224));

            var data = new DenseTensor<float>(new[] { 1, 224, 224, 3 });
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = image[x, y];
                    data[0, y, x, 0] = pixel.R / 255f;
                    data[0, y, x, 1] = pixel.G / 255f;
                    data[0, y, x, 2] = pixel.B / 255f;
                }
            }

            return data;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
