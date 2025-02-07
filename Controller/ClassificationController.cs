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

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? number)
        {
            // ✅ Check if input is missing or empty
            if (string.IsNullOrWhiteSpace(number))
            {
                return BadRequest(new { number = "missing", error = true });
            }

            // ✅ Trim spaces and validate as an integer
            if (!int.TryParse(number.Trim(), out int validatedNumber))
            {
                return BadRequest(new { number, error = true });
            }

            // Compute mathematical properties
            bool isPrime = IsPrime(validatedNumber);
            bool isPerfect = IsPerfect(validatedNumber);
            int digitSum = GetDigitSum(validatedNumber);
            bool isArmstrong = IsArmstrong(validatedNumber);

            // Determine properties
            var properties = new List<string>();
            if (isArmstrong) properties.Add("armstrong");
            properties.Add(validatedNumber % 2 == 0 ? "even" : "odd");

            // ✅ Fetch fun fact
            var funFactResponse = await GetFunFact(validatedNumber);
            string funFact = funFactResponse?.text ?? "No fact available";

            return Ok(new
            {
                number = validatedNumber,
                is_prime = isPrime,
                is_perfect = isPerfect,
                properties,
                digit_sum = digitSum,
                fun_fact = funFact
            });
        }

        private bool IsArmstrong(int num)
        {
            int absNum = Math.Abs(num);
            int sum = 0, nDigits = absNum.ToString().Length, temp = absNum;

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
            if (num <= 1) return false;
            for (int i = 2; i <= Math.Sqrt(num); i++)
            {
                if (num % i == 0) return false;
            }
            return true;
        }

        private bool IsPerfect(int num)
        {
            if (num <= 0) return false;
            int sum = 1;
            for (int i = 2; i <= Math.Sqrt(num); i++)
            {
                if (num % i == 0)
                {
                    sum += i;
                    if (i != num / i) sum += num / i;
                }
            }
            return sum == num && num != 1;
        }

        private int GetDigitSum(int num)
        {
            int sum = 0;
            while (num > 0)
            {
                sum += num % 10;
                num /= 10;
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

