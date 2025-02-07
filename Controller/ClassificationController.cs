using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClassifyNumber.Controllers
{
    [Route("api/classify-number")]
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
            public string? text { get; set; }
        }

        // GET: api/classify-number?number=371
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string number)
        {
            // Validate input: if null/empty or not an integer, return error response.
            if (string.IsNullOrWhiteSpace(number) || !int.TryParse(number, out int num))
            {
                return BadRequest(new { number, error = true });
            }

            // Compute mathematical properties.
            bool isPrime = IsPrime(num);
            bool isPerfect = IsPerfect(num);
            int digitSum = GetDigitSum(num);

            // Determine properties field based on Armstrong and parity.
            var properties = new List<string>();
            bool isArmstrong = IsArmstrong(num);
            if (isArmstrong)
            {
                properties.Add("armstrong");
            }
            properties.Add(num % 2 == 0 ? "even" : "odd");

            // Fetch fun fact from Numbers API (using math type)
            var funFactResponse = await GetFunFact(num);
            string funFact = funFactResponse?.text ?? "No fact available";

            // Return the JSON response in the required format.
            return Ok(new
            {
                number = num,
                is_prime = isPrime,
                is_perfect = isPerfect,
                properties,
                digit_sum = digitSum,
                fun_fact = funFact
            });
        }

        private bool IsArmstrong(int num)
        {
            // Use the absolute value for checking Armstrong status.
            int absNum = Math.Abs(num);
            int sum = 0;
            int nDigits = absNum.ToString().Length;
            int temp = absNum;
            while (temp > 0)
            {
                int digit = temp % 10;
                sum += (int)Math.Pow(digit, nDigits);
                temp /= 10;
            }
            return sum == absNum;
        }

        private bool IsPrime(int num)
        {
            // Only numbers greater than 1 can be prime.
            if (num <= 1)
                return false;
            int absNum = Math.Abs(num);
            for (int i = 2; i <= Math.Sqrt(absNum); i++)
            {
                if (absNum % i == 0)
                    return false;
            }
            return true;
        }

        private bool IsPerfect(int num)
        {
            // Perfect numbers are positive and equal the sum of their proper divisors.
            if (num <= 0)
                return false;
            int sum = 1;
            for (int i = 2; i <= Math.Sqrt(num); i++)
            {
                if (num % i == 0)
                {
                    sum += i;
                    if (i != num / i)
                    {
                        sum += num / i;
                    }
                }
            }
            // Exclude 1 from being perfect.
            return sum == num && num != 1;
        }

        private int GetDigitSum(int num)
        {
            int absNum = Math.Abs(num);
            int sum = 0;
            while (absNum > 0)
            {
                sum += absNum % 10;
                absNum /= 10;
            }
            return sum;
        }

        private async Task<FunFactResponse?> GetFunFact(int num)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"http://numbersapi.com/{num}/math?json");
                return JsonConvert.DeserializeObject<FunFactResponse>(response);
            }
            catch
            {
                return new FunFactResponse { text = "No fact available" };
            }
        }
    }
}

