using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ClassifyNumber.Controllers
{
    [Route("api/classify-number")] // Route to access this controller
    [ApiController]
    public class ClassificationController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ClassificationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public class FunFactResponse
        {
            public string? text { get; set; } = string.Empty;
            public int number { get; set; }
            public bool found { get; set; }
            public string? type { get; set; } = string.Empty;
        }

        public class NumberClassificationRequest
        {
            public string NumberString { get; set; } = string.Empty;
        }

        // GET: api/classify-number?numberString=5
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string numberString)
        {
            return await ClassifyNumber(numberString);
        }

        // POST: api/classify-number
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NumberClassificationRequest request)
        {
            return await ClassifyNumber(request.NumberString);
        }

        private async Task<IActionResult> ClassifyNumber(string numberString)
        {
            // Validate the input as a valid integer
            if (string.IsNullOrWhiteSpace(numberString) ||
                !int.TryParse(numberString, out int number))
            {
                return BadRequest(new { error = true, message = "Invalid or missing number" });
            }

            if (number < 0)
            {
                return BadRequest(new { error = true, message = "Number must be non-negative" });
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
                is_perfect = IsPerfect(number),
                properties,
                digit_sum = digitSum,
                fun_fact = funFact.text
            };

            return Ok(response);
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

        private bool IsPerfect(int n)
        {
            int sum = 0;
            for (int i = 1; i <= n / 2; i++)
            {
                if (n % i == 0)
                {
                    sum += i;
                }
            }
            return sum == n && n != 0;
        }

        private int GetDigitSum(int n)
        {
            return n.ToString().Select(d => int.Parse(d.ToString())).Sum();
        }

        private async Task<FunFactResponse> GetFunFact(int number, bool isArmstrong)
        {
            // Choose the appropriate type for fun fact
            string type = isArmstrong ? "math" : "trivia";
            var response = await _httpClient.GetStringAsync($"http://numbersapi.com/{number}/{type}?json");

            // Deserialize and return the fun fact response
            return JsonConvert.DeserializeObject<FunFactResponse>(response) ?? new FunFactResponse();
        }
    }
}
