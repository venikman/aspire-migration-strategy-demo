using AspireReact.Api;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (OpenTelemetry, health checks, service discovery)
builder.AddServiceDefaults();

// Add OpenAPI/Swagger support
builder.Services.AddOpenApi();

// Add CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();

// Map OpenAPI endpoint for development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Map Aspire health check endpoints
app.MapDefaultEndpoints();

// Sample weather API endpoint
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async (CancellationToken cancellationToken) =>
{
    // Create a custom span for the weather forecast generation
    // This demonstrates manual instrumentation for business logic
    using var activity = AspireReact.Api.Telemetry.ActivitySource.StartActivity("GenerateWeatherForecast");

    // Add custom tags to provide context about this operation
    activity?.SetTag("forecast.days", 5);
    activity?.SetTag("forecast.unit", "celsius");

    // Simulate data fetching with a custom span
    var forecastData = await FetchForecastDataAsync(summaries, cancellationToken);

    // Add an event to the span (useful for debugging or marking important moments)
    activity?.AddEvent(new System.Diagnostics.ActivityEvent("ForecastGenerated",
        tags: new System.Diagnostics.ActivityTagsCollection
        {
            { "forecast.count", forecastData.Length }
        }));

    return forecastData;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// Demonstrate baggage propagation (context that flows across service boundaries)
app.MapGet("/weatherforecast/{city}", async (string city, CancellationToken cancellationToken) =>
{
    using var activity = AspireReact.Api.Telemetry.ActivitySource.StartActivity("GetWeatherByCity");

    // Add baggage - this propagates to downstream services but doesn't appear in the current span
    // Useful for correlation IDs, user IDs, feature flags, etc.
    System.Diagnostics.Activity.Current?.AddBaggage("city", city);
    System.Diagnostics.Activity.Current?.AddBaggage("request.id", Guid.NewGuid().ToString());

    // Baggage can be read by downstream services or in the current request
    var cityFromBaggage = System.Diagnostics.Activity.Current?.GetBaggageItem("city");
    activity?.SetTag("location.city", cityFromBaggage);

    // Simulate city-specific weather data
    await Task.Delay(Random.Shared.Next(10, 50), cancellationToken);

    var forecast = new WeatherForecast(
        DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
        Random.Shared.Next(-20, 55),
        summaries[Random.Shared.Next(summaries.Length)]
    );

    activity?.AddEvent(new System.Diagnostics.ActivityEvent("CityWeatherFetched",
        tags: new System.Diagnostics.ActivityTagsCollection
        {
            { "city", city },
            { "temperature", forecast.TemperatureC }
        }));

    return forecast;
})
.WithName("GetWeatherByCity")
.WithOpenApi();

// Demonstrate custom instrumentation in a helper method
static async Task<WeatherForecast[]> FetchForecastDataAsync(string[] summaries, CancellationToken cancellationToken)
{
    // Create a child span for the data fetching operation
    using var activity = AspireReact.Api.Telemetry.ActivitySource.StartActivity("FetchWeatherData");

    try
    {
        // Simulate async work (e.g., database query, external API call)
        await Task.Delay(Random.Shared.Next(10, 50), cancellationToken);

        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast(
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();

        // Add tags after the operation completes
        activity?.SetTag("data.source", "random-generator");
        activity?.SetTag("data.records", forecast.Length);

        return forecast;
    }
    catch (Exception ex)
    {
        // Record exceptions in the span
        activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
        activity?.RecordException(ex);
        throw;
    }
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
