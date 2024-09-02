using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureAi_Lab2.Services
{
	public class CustomVisionService
	{
		private readonly CustomVisionPredictionClient _predictionClient;
		private readonly string _projectId;
		private readonly string _iterationName;

		public CustomVisionService()
		{
			var endpoint = Environment.GetEnvironmentVariable("AZURE_CUSTOM_VISION_ENDPOINT");
			var key = Environment.GetEnvironmentVariable("AZURE_CUSTOM_VISION_KEY");
			_projectId = Environment.GetEnvironmentVariable("AZURE_CUSTOM_VISION_PROJECT_ID");
			_iterationName = Environment.GetEnvironmentVariable("AZURE_CUSTOM_VISION_ITERATION_NAME");

			_predictionClient = new CustomVisionPredictionClient(new ApiKeyServiceClientCredentials(key))
			{
				Endpoint = endpoint 
			};
		}

		public async Task<ImagePrediction> PredictMushroomTypeAsync(Stream imageStream)
		{
			try
			{
				return await _predictionClient.ClassifyImageAsync(
					new Guid(_projectId),
					_iterationName,
					imageStream);
			}
			catch (CustomVisionErrorException ex)
			{
				Console.WriteLine($"Custom Vision API error: {ex.Body?.Code} - {ex.Body?.Message}");
				throw;
			}
		}
	}
}