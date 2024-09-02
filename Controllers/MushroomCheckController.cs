using AzureAi_Lab2.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzureAi_Lab2.Controllers
{
	public class MushroomCheckController : Controller
	{
		private readonly CustomVisionService _customVisionService;

		public MushroomCheckController(CustomVisionService customVisionService)
		{
			_customVisionService = customVisionService;
		}

		[HttpGet]
		public IActionResult UploadImage()
		{
			return View();
		}

		[HttpGet]
		public IActionResult PredictionResult()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Predict(IFormFile imageFile)
		{
			try
			{
				if (imageFile == null || imageFile.Length == 0)
				{
					ViewBag.Message = "You need to add an image";
					return View(nameof(UploadImage));
				}

				// Convert the uploaded image to a stream
				using var memoryStream = new MemoryStream();
				await imageFile.CopyToAsync(memoryStream);
				memoryStream.Position = 0; // Reset stream position to the beginning

				// Call the Custom Vision service to get predictions
				var predictionResult = await _customVisionService.PredictMushroomTypeAsync(memoryStream);

				// Extract the highest probability prediction
				var topPrediction = predictionResult.Predictions.OrderByDescending(p => p.Probability).FirstOrDefault();
				if (topPrediction != null)
				{
					ViewBag.PredictionLabel = topPrediction.TagName;
					ViewBag.Probability = topPrediction.Probability.ToString("P");
				}
				else
				{
					ViewBag.Message = "No predictions could be made.";
					return View(nameof(UploadImage));
				}

				// Save the image to display it
				var imagePath = Path.Combine("wwwroot/images/checked-mushrooms", imageFile.FileName);
				using (var stream = new FileStream(imagePath, FileMode.Create))
				{
					await imageFile.CopyToAsync(stream);
				}
				ViewBag.ImageName = imageFile.FileName;
				ViewBag.ImagePath = "/images/checked-mushrooms/" + imageFile.FileName;

				return View(nameof(PredictionResult));
			}
			catch (Exception ex)
			{
				// Handle exceptions and show an error message
				ViewBag.Message = $"An error occurred: {ex.Message}";
				return View(nameof(UploadImage));
			}
		}
	}
}
