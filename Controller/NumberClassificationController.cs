using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HngBackendNumberClassifier.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NumberClassificationController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public NumberClassificationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // FunFactResponse defined within the controller
        public class FunFactResponse
        {
            public string? text { get; set; } = string.Empty;
            public int number { get; set; }
            public bool found { get; set; }
            public string? type { get; set; } = string.Empty;
        }

        // GET: api/NumberClassification?number=5
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int number)
        {
            // Validate the number
            if (number < 0)
            {
                return BadRequest(new { error = true, message = "Invalid number" });
            }

            var properties = new List<string>();

            // Check if the number is an Armstrong number
            bool isArmstrong = IsArmstrong(number);
            if (isArmstrong)
            {
                properties.Add("armstrong");
            }

            // Determine if the number is odd or even
            if (number % 2 == 0)
            {
                properties.Add("even");
            }
            else
            {
                properties.Add("odd");
            }

            // Get the sum of digits
            int digitSum = GetDigitSum(number);

            // Fetch fun fact from Numbers API
            var funFact = await GetFunFact(number, isArmstrong);

            // Create response object
            var response = new
            {
                number,
                is_prime = IsPrime(number),
                is_perfect = false, // Placeholder, implement if required
                properties,
                digit_sum = digitSum,
                fun_fact = funFact
            };

            return Ok(response);
        }
        private int GetRandomNumber()
        {
            Random random = new Random();
            return random.Next(1, 101);
        }

        private bool IsArmstrong(int n)
        {
            int numDigits = n.ToString().Length;
            int sum = n.ToString()
                .Select(d => (int)Math.Pow(int.Parse(d.ToString()), numDigits))
                .Sum();
            return sum == n;
        }

        private bool IsPrime(int n)
        {
            if (n < 2) return false;
            for (int i = 2; i <= Math.Sqrt(n); i++)
            {
                if (n % i == 0) return false;
            }
            return true;
        }

        private int GetDigitSum(int n)
        {
            return n.ToString().Select(d => int.Parse(d.ToString())).Sum();
        }

        private async Task<FunFactResponse> GetFunFact(int number, bool isArmstrong)
        {
            // Choose the appropriate type for fun fact based on whether it's Armstrong or not
            string type = isArmstrong ? "math" : "trivia";
            var response = await _httpClient.GetStringAsync($"http://numbersapi.com/{number}/{type}?json");

            // Deserialize and return the fun fact response
            return JsonConvert.DeserializeObject<FunFactResponse>(response) ?? new FunFactResponse();
        }
    }
}
