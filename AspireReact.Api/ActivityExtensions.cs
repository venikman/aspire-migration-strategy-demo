using System.Diagnostics;

namespace AspireReact.Api;

/// <summary>
/// Extension methods for System.Diagnostics.Activity to support OpenTelemetry semantic conventions.
/// </summary>
public static class ActivityExtensions
{
    /// <summary>
    /// Records an exception in the current activity following OpenTelemetry semantic conventions.
    /// This adds exception details as span events with standardized attribute names.
    /// </summary>
    /// <param name="activity">The activity to record the exception on.</param>
    /// <param name="exception">The exception to record.</param>
    public static void RecordException(this Activity? activity, Exception exception)
    {
        if (activity == null || exception == null)
            return;

        var tags = new ActivityTagsCollection
        {
            { "exception.type", exception.GetType().FullName },
            { "exception.message", exception.Message },
            { "exception.stacktrace", exception.ToString() }
        };

        activity.AddEvent(new ActivityEvent("exception", tags: tags));
    }
}
