using System.Diagnostics;

namespace AspireReact.Api;

/// <summary>
/// Centralized telemetry configuration for custom instrumentation.
/// ActivitySource instances should be static and created once per application.
/// </summary>
public static class Telemetry
{
    /// <summary>
    /// The name should match what's registered in ServiceDefaults.
    /// This allows OpenTelemetry to collect spans from this source.
    /// </summary>
    public const string ActivitySourceName = "AspireReact.Api";

    /// <summary>
    /// ActivitySource for creating custom spans in business logic.
    /// Use this to trace operations that aren't automatically instrumented (e.g., database calls, external APIs, business logic).
    /// </summary>
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName, "1.0.0");
}
