using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Enable controllers

// Register HttpClient service in dependency injection
builder.Services.AddHttpClient(); // This enables the HttpClient to be injected into your controllers

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Show detailed error messages in development
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Redirect to error page in production
    app.UseHsts(); // Apply HTTP Strict Transport Security
}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseStaticFiles(); // Serve static files

app.UseRouting(); // Enable routing

app.UseAuthorization(); // Enable authorization

app.MapControllers(); // Map attribute-routed controllers

// Run the application
app.Run();
