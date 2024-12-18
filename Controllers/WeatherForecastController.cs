using Microsoft.AspNetCore.Mvc;

namespace Net8APISample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("long-running-operation")]
        public async Task<IActionResult> LongRunningOperation(CancellationToken cancellationToken)
        {
            int iterations = 10; // Number of iterations for the long-running operation
            for (int i = 0; i < iterations; i++)
            {
                // Check if the request has been canceled
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine($"Request was canceled in iteration {i + 1}.");
                    return StatusCode(499, "Request canceled by the client."); // 499: Client Closed Request
                }

                Console.WriteLine($"Iteration {i + 1} of {iterations}... (Server is processing)");

                // Simulate some work with a delay
                try
                {
                    await Task.Delay(1000, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine($"TaskCanceledException caught in iteration {i + 1}.");
                    return StatusCode(499, "Request canceled by the client.");
                }
            }

            Console.WriteLine("Operation completed successfully.");
            return Ok("Operation completed successfully.");
        }
    }


}
