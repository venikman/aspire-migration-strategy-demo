using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Http.Polly;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Extensions.Http;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Extension methods for configuring common .NET Aspire 9.5 services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds common .NET Aspire services: service discovery, resilience (manual Polly), health checks, and OpenTelemetry.
    /// </summary>
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        builder.ConfigureOpenTelemetry();
        builder.AddDefaultHealthChecks();
        builder.Services.AddServiceDiscovery();

        // ⚠️  Aspire 9.5: Manual Polly configuration required (~50 lines)
        // Compare to Aspire 13: http.AddStandardResilienceHandler(); (1 line)
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddServiceDiscovery();

            // Retry policy: 3 attempts with exponential backoff
            http.AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        var logger = context.GetLogger();
                        logger?.LogWarning("Retry {RetryCount} after {Delay}ms due to: {Exception}",
                            retryCount, timespan.TotalMilliseconds, outcome.Exception?.Message);
                    }));

            // Circuit breaker policy: Open after 5 failures, half-open after 30s
            http.AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, duration, context) =>
                    {
                        var logger = context.GetLogger();
                        logger?.LogError("Circuit breaker opened for {Duration}s due to: {Exception}",
                            duration.TotalSeconds, outcome.Exception?.Message);
                    },
                    onReset: context =>
                    {
                        var logger = context.GetLogger();
                        logger?.LogInformation("Circuit breaker reset");
                    }));

            // Timeout policy: 30 seconds
            http.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30)));
        });

        return builder;
    }

    /// <summary>
    /// Configures OpenTelemetry for logging, tracing, and metrics.
    /// </summary>
    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: builder.Environment.ApplicationName,
                    serviceVersion: typeof(Extensions).Assembly.GetName().Version?.ToString() ?? "1.0.0",
                    serviceInstanceId: Environment.MachineName)
                .AddAttributes(new[]
                {
                    new KeyValuePair<string, object>("deployment.environment", builder.Environment.EnvironmentName),
                    new KeyValuePair<string, object>("host.name", Environment.MachineName),
                    new KeyValuePair<string, object>("service.namespace", "aspire-react"),
                }))
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                // ⚠️  Aspire 9.5: Static 100% sampling (expensive in production!)
                // Compare to Aspire 13: Environment-based sampling (10% prod, 100% dev)
                tracing.SetSampler(new AlwaysOnSampler());
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        return builder;
    }

    /// <summary>
    /// Adds default health checks for the application.
    /// </summary>
    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());

        return builder;
    }

    /// <summary>
    /// Maps health check endpoints (Aspire 9.5 - single endpoint only).
    /// Compare to Aspire 13: Separate /health and /alive for Kubernetes probes.
    /// </summary>
    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // ⚠️  Aspire 9.5: Single health check endpoint
        // No separation between liveness and readiness probes
        app.MapHealthChecks("/health");

        return app;
    }

    // Helper extension for getting logger from Polly context
    private static ILogger? GetLogger(this Polly.Context context)
    {
        if (context.TryGetValue("logger", out var loggerObj) && loggerObj is ILogger logger)
        {
            return logger;
        }
        return null;
    }
}
