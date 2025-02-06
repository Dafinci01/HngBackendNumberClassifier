  using Microsoft.AspNetCore.Mvc;
  using System;
  using System.Linq;
  using System.Net.Http;
  using System.Threading.Tasks;
  using System.Collections.Generic;
  using Newtonsoft.Json;

  namespace NumberClassificationApi.Controllers
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

          // GET: api/NumberClassification?number=5
          [HttpGet]
          public async Task<IActionResult> Get(int number)
          {
              if (number < 0)
              {
                  return BadRequest(new { error = true, message = "Invalid number" });
              }

              var properties = new List<string>();

              if (IsArmstrong(number))
              {
                  properties.Add("armstrong");
              }

              properties.Add(number % 2 == 0 ? "even" : "odd");

              var response = new
              {
                  number,
                  is_prime = IsPrime(number),
                  is_perfect = IsPerfect(number),
                  properties,
                  digit_sum = number.ToString().Sum(c => c - '0'),
                  fun_fact = await FetchFunFactAsync(number)
              };

              // Deserialize the fun_fact string into a JSON object
              var funFact = JsonConvert.DeserializeObject(response.fun_fact.ToString());

              // Return the response with the deserialized fun_fact
              return Ok(new
              {
                  response.number,
                  response.is_prime,
                  response.is_perfect,
                  response.properties,
                  response.digit_sum,
                  fun_fact = funFact
              });
          }

          private bool IsPrime(int n)
          {
              if (n <= 1) return false;
              for (int i = 2; i <= Math.Sqrt(n); i++)
              {
                  if (n % i == 0) return false;
              }
              return true;
          }

          private bool IsArmstrong(int n)
          {
              var digits = n.ToString().Select(c => int.Parse(c.ToString())).ToArray();
              var numDigits = digits.Length;
              var sum = digits.Sum(d => Math.Pow(d, numDigits));
              return sum == n;
          }

          private bool IsPerfect(int n)
          {
              if (n <= 0) return false;
              var sum = Enumerable.Range(1, n / 2).Where(i => n % i == 0).Sum();
              return sum == n;
          }

          private async Task<string> FetchFunFactAsync(int n)
          {
              try
              {
                  var response = await _httpClient.GetStringAsync($"http://numbersapi.com/{n}/math?json");
                  return response ?? $"No fact found for {n}";
              }
              catch
              {
                  return $"Could not fetch fun fact for {n}.";
              }
          }
      }
  }                                                                                                                                                                                                                                                                              ~                                                                                                                                                                                                                                                                               ~
