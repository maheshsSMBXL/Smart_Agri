using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using SixLabors.ImageSharp.PixelFormats;
using Trees_RaysApi.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Agri_Smart.Services;

namespace Agri_Smart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictionController : ControllerBase
    {
        private readonly InferenceSession _session;
        private readonly InferenceSession _leafSessioin;
        private readonly ILogger<PredictionController> _logger;
        private readonly FlaskApiService _flaskApiService;
        private readonly InferenceSession _pestDiseaseSession;

        public PredictionController(ILogger<PredictionController> logger, IWebHostEnvironment env, FlaskApiService flaskApiService)
        {
            _logger = logger;
            var modelPath = Path.Combine(env.ContentRootPath, "ML_Models", "coffee_leaf_model.onnx");
            _session = new InferenceSession(modelPath);
            var LeafDetectionPath = Path.Combine(env.ContentRootPath, "ML_Models", "leaf_classification_model.onnx");
            _leafSessioin = new InferenceSession(LeafDetectionPath);
            _flaskApiService = flaskApiService;
            var pestDiseaseModelPath = Path.Combine(env.ContentRootPath, "ML_Models", "pest_and_disease_model.onnx");
            _pestDiseaseSession = new InferenceSession(pestDiseaseModelPath);
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

        //[HttpPost("LeafSegmentation")]
        //public async Task<IActionResult> LeafSegmentation(IFormFile image)
        //{
        //    if (image == null || image.Length == 0)
        //        return BadRequest("Image file is missing");

        //    var filePath = Path.GetTempFileName();
        //    using (var stream = System.IO.File.Create(filePath))
        //    {
        //        await image.CopyToAsync(stream);
        //    }

        //    var result = await _flaskApiService.PredictLeafDiseaseAsync(filePath);
        //    System.IO.File.Delete(filePath);  // Clean up the temp file
        //    return Ok(result);
        //}

        //public IActionResult Index()
        //{
        //    return View();





        // Pest and Disease Prediction
        [HttpPost("PestDiseasePrediction")]
        public async Task<IActionResult> PestDiseasePrediction(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded." });
            }

            // Preprocess the uploaded image
            var inputData = PreprocessImageFile(file, 224, 224);
            var tensor = new DenseTensor<float>(inputData, new[] { 1, 224, 224, 3 });

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", tensor)
            };

            try
            {
                // Run inference with the model
                using var results = _pestDiseaseSession.Run(inputs);
                var output = results.First().AsEnumerable<float>().ToArray();
                int predictedClassIndex = Array.IndexOf(output, output.Max());

                // Define classes (adjust according to your model)
                string[] diseaseClassNames = { "Cercospora", "Healthy", "Leaf rust", "Miner", "Phoma" };
                string[] pestClassNames = { "Coffee Berry Borer", "Mealybugs", "Short Hole Borer", "White Stem Borer" };

                string predictedLabel;
                string description;
                string predictionType;

                // Determine if the prediction is a disease or a pest
                if (predictedClassIndex < diseaseClassNames.Length)
                {
                    predictedLabel = diseaseClassNames[predictedClassIndex];
                    description = GetDiseaseDescription(predictedLabel);
                    predictionType = "disease";
                }
                else
                {
                    predictedLabel = pestClassNames[predictedClassIndex - diseaseClassNames.Length];
                    description = GetPestDescription(predictedLabel);
                    predictionType = "pest";
                }

                // Return prediction response
                return Ok(new
                {
                    prediction = predictedLabel,
                    description,
                    type = predictionType
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during pest/disease prediction.");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        // Preprocess the image for ONNX model input
        private float[] PreprocessImageFile(IFormFile file, int targetWidth, int targetHeight)
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

        // Helper methods for descriptions
        private string GetDiseaseDescription(string diseaseName)
        {
            return diseaseName switch
            {
                "Cercospora" => "A fungal disease that causes brown spots on leaves.",
                "Healthy" => "No disease or pests detected. The plant appears healthy.",
                "Leaf rust" => "A common fungal disease causing rust-colored spots on leaves.",
                "Miner" => "A pest that tunnels through leaves, creating pale trails.",
                "Phoma" => "A fungal disease causing black spots on stems and leaves.",
                _ => "No description available."
            };
        }

        private string GetPestDescription(string pestName)
        {
            return pestName switch
            {
                "Coffee Berry Borer" => "A small beetle that burrows into coffee berries, damaging the fruit.",
                "Mealybugs" => "Small insects that feed on plant sap, weakening the plant.",
                "Short Hole Borer" => "A pest that bores holes into coffee trees, causing damage to branches.",
                "White Stem Borer" => "A serious pest that tunnels into the stems, leading to plant wilting.",
                _ => "No description available."
            };
        }
    }
}
