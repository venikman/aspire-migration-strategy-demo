# AspireReact - Full-Stack Observability with .NET Aspire & OpenTelemetry

A production-ready example of end-to-end distributed tracing in a modern web application using .NET Aspire, React, and OpenTelemetry.

Distributed Tracing Architecture - Hero Banner

---

## 🌟 What is .NET Aspire?

**Never heard of .NET Aspire?** You're not alone! Let's start with the basics.

**.NET Aspire** is Microsoft's opinionated stack for building **cloud-native, distributed applications**. Think of it as a "batteries-included" framework that takes care of all the boring infrastructure code you'd normally write yourself.

### The Problem Aspire Solves

Traditionally, when building a modern web app with multiple services (API, frontend, database, cache, etc.), you'd need to:

```
❌ Manually configure logging for each service
❌ Set up distributed tracing (OpenTelemetry)
❌ Write Docker Compose files
❌ Configure health checks
❌ Set up service discovery
❌ Add retry logic, circuit breakers, timeouts
❌ Wire up a dashboard to view logs/traces
❌ Manage environment variables across services
❌ Handle development vs production configurations
```

**This is tedious, error-prone, and repetitive across every project.**

### The Aspire Solution

Aspire provides:

1. **Service Orchestration** (`AppHost`)
   - Declare your app's architecture in C# code
   - One `dotnet run` starts everything (API, frontend, databases, etc.)
   - No Docker Compose YAML, no manual port management

2. **Automatic Observability** (`ServiceDefaults`)
   - Logging, tracing, metrics configured out-of-the-box
   - OpenTelemetry integration with zero boilerplate
   - Built-in dashboard to view all telemetry data

3. **Built-in Resilience**
   - Retry policies, circuit breakers, timeouts with one line of code
   - Production-ready patterns without manual Polly configuration

4. **Service Discovery**
   - Services find each other automatically (no hardcoded URLs)
   - Works locally and in production (Kubernetes, Azure Container Apps)

### What This Project Demonstrates

This repository shows a **real-world implementation** of .NET Aspire 13 with:
- **Backend**: ASP.NET Core Minimal API with custom OpenTelemetry instrumentation
- **Frontend**: React + Vite with browser-based tracing
- **Observability**: End-to-end distributed tracing from browser to API
- **Production Patterns**: Environment-based sampling, health checks, resilience policies

**New to Aspire?** Follow the [Getting Started](#-getting-started) section below - you'll have a full-stack app with observability running in under 5 minutes!

---

## 🎯 What's Demonstrated

This project showcases:
- **End-to-End Tracing**: Browser → API distributed traces with W3C Trace Context propagation
- **Automatic Instrumentation**: ASP.NET Core, HttpClient, Fetch API, Document Load, User Interactions
- **Custom Instrumentation**: Business logic spans, tags, events, error handling
- **Resource Attributes**: Service identification with name, version, environment
- **Sampling Strategies**: Environment-based sampling (100% dev, 10% production)
- **Baggage Propagation**: Cross-service context sharing
- **.NET Aspire Integration**: Service defaults, health checks, dashboard

---

## 📁 Project Structure

```
AspireReact/
├── AspireReact.AppHost/         # Aspire orchestration
│   └── AppHost.cs               # Defines API + React topology
├── AspireReact.Api/             # C# Minimal API backend
│   ├── Program.cs               # Endpoints with custom instrumentation
│   ├── Telemetry.cs             # ActivitySource configuration
│   └── ActivityExtensions.cs   # OpenTelemetry helpers
├── AspireReact.React/           # Vite + React + TypeScript frontend
│   ├── src/
│   │   ├── telemetry.ts         # Browser OpenTelemetry setup
│   │   ├── main.tsx             # Initializes telemetry before render
│   │   └── App.tsx              # Weather forecast UI
│   └── .env.development         # OTLP endpoint configuration
└── AspireReact.ServiceDefaults/ # Shared observability config
    └── Extensions.cs            # OTel, health checks, service discovery
```

*Architectural layers - from infrastructure foundation to observability dashboard*

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/) (for React frontend)
- [.NET Aspire Workload](https://learn.microsoft.com/dotnet/aspire/fundamentals/setup-tooling)

```bash
# Install Aspire workload
dotnet workload update
dotnet workload install aspire
```

### Run the Application

```bash
# 1. Install frontend dependencies
cd AspireReact.React
npm install

# 2. Run the Aspire AppHost (starts API + React + Dashboard)
cd ../AspireReact.AppHost
dotnet run
```

*Single command orchestration - AppHost starts all services with automatic configuration*

### Access Points
- **Aspire Dashboard**: https://localhost:21178 (view traces, metrics, logs)
- **API**: http://localhost:5100
- **React Frontend**: http://localhost:3000
- **Health Checks**: http://localhost:5100/health

---

## 🔍 Observability Architecture

*End-to-end trace propagation - Browser to API with W3C Trace Context headers*

### Frontend (React + OpenTelemetry Web SDK)

**Location**: `AspireReact.React/src/telemetry.ts`

```typescript
// Automatic instrumentation for:
// 1. Fetch API calls (including W3C Trace Context propagation)
// 2. Document load events (page performance)
// 3. User interactions (clicks, form submissions)

initializeTelemetry(); // Called in main.tsx before React renders
```

**Key Features**:
- **FetchInstrumentation**: Traces all `fetch()` calls, adds `traceparent` header
- **W3C Trace Context**: Propagates trace ID to backend for distributed tracing
- **OTLP Exporter**: Sends spans to Aspire Dashboard (http://localhost:4318/v1/traces)
- **Resource Attributes**: `service.name=aspire-react-frontend`, `service.version=1.0.0`

**Environment Variables** (`.env.development`):
```env
VITE_OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4318/v1/traces
```

### Backend (ASP.NET Core + OpenTelemetry .NET SDK)

**Location**: `AspireReact.ServiceDefaults/Extensions.cs`

```csharp
builder.AddServiceDefaults(); // Adds OTel, health checks, service discovery
```

**ServiceDefaults Includes**:
1. **Logging**: OpenTelemetry logs with formatted messages & scopes
2. **Metrics**: AspNetCore, HttpClient, Runtime instrumentation
3. **Tracing**: AspNetCore, HttpClient with custom ActivitySource support
4. **Resource Attributes**: Service name, version, environment, host
5. **Sampling**: AlwaysOn (dev) | ParentBased+TraceIdRatio 10% (prod)
6. **Health Checks**: `/health` (general), `/alive` (liveness probe)

### Custom Instrumentation Examples

**1. Basic Custom Span** (`AspireReact.Api/Program.cs:30-50`)
```csharp
using var activity = Telemetry.ActivitySource.StartActivity("GenerateWeatherForecast");
activity?.SetTag("forecast.days", 5);
activity?.SetTag("forecast.unit", "celsius");
```

**2. Child Spans** (`AspireReact.Api/Program.cs:91-122`)
```csharp
using var activity = Telemetry.ActivitySource.StartActivity("FetchWeatherData");
// Child span automatically nests under current Activity.Current
```

**3. Events** (Timeline markers within a span)
```csharp
activity?.AddEvent(new ActivityEvent("ForecastGenerated",
    tags: new ActivityTagsCollection { { "forecast.count", 5 } }));
```

**4. Error Handling**
```csharp
catch (Exception ex)
{
    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
    activity?.RecordException(ex); // Custom extension method
    throw;
}
```

**5. Baggage Propagation** (`AspireReact.Api/Program.cs:56-88`)
```csharp
// Add context that flows to downstream services (not visible on current span)
Activity.Current?.AddBaggage("city", city);
Activity.Current?.AddBaggage("request.id", Guid.NewGuid().ToString());

// Read baggage in the same request or downstream services
var cityFromBaggage = Activity.Current?.GetBaggageItem("city");
```

---

## 📊 Viewing Telemetry Data

### Aspire Dashboard (Recommended)
1. Run `dotnet run` in `AspireReact.AppHost`
2. Open https://localhost:21178
3. Navigate to **Traces** tab
4. Click on a trace to see:
   - Frontend span: `HTTP GET /api/weatherforecast`
   - Backend span: `aspnetcore-http-in /weatherforecast`
   - Custom spans: `GenerateWeatherForecast`, `FetchWeatherData`

### Trace Flow Example
```
Browser (aspire-react-frontend)
└─ HTTP GET /api/weatherforecast [FetchInstrumentation]
   └─ API (AspireReact.Api)
      └─ aspnetcore-http-in GET /weatherforecast [Auto]
         └─ GenerateWeatherForecast [Custom]
            └─ FetchWeatherData [Custom]
               ├─ Event: ForecastGenerated
               └─ Tags: data.source=random-generator
```

---

## 🛠️ Adding Custom Instrumentation to Your Code

### Step 1: Create an ActivitySource

```csharp
// In your project (e.g., MyProject/Telemetry.cs)
public static class Telemetry
{
    public const string ActivitySourceName = "MyProject";
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName, "1.0.0");
}
```

### Step 2: Register the ActivitySource in ServiceDefaults

The ActivitySource name must match what's registered in `ConfigureOpenTelemetry()`:

```csharp
// Already configured in ServiceDefaults/Extensions.cs:65
tracing.AddSource(builder.Environment.ApplicationName)
```

For additional sources:
```csharp
tracing.AddSource("MyProject")
```

### Step 3: Create Spans in Your Code

```csharp
using var activity = Telemetry.ActivitySource.StartActivity("MyOperation");
activity?.SetTag("user.id", userId);
activity?.SetTag("operation.type", "data-processing");

try
{
    // Your business logic here
    var result = await ProcessDataAsync();
    activity?.AddEvent(new ActivityEvent("ProcessingCompleted"));
    return result;
}
catch (Exception ex)
{
    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
    activity?.RecordException(ex);
    throw;
}
```

### Best Practices

**DO**:
- ✅ Use `using` statements to ensure spans close
- ✅ Add meaningful tags (filterable metadata)
- ✅ Record exceptions with `RecordException()`
- ✅ Use child spans for sub-operations
- ✅ Set status on errors: `SetStatus(ActivityStatusCode.Error)`

**DON'T**:
- ❌ Create too many spans (max ~100 per trace)
- ❌ Add PII to tags (user emails, passwords, credit cards)
- ❌ Use high-cardinality tags (UUIDs, timestamps) - breaks aggregation
- ❌ Forget to check `activity != null` (may be null if sampling drops it)
- ❌ Block on async operations inside spans

---

## 🧪 Testing the Telemetry

### 1. Verify Frontend Instrumentation
Open browser DevTools → Network tab:
```
Request Headers:
traceparent: 00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01
           └─ version trace-id            parent-id            flags
```

### 2. Verify Backend Receives Context
Check Aspire Dashboard → Traces:
- Frontend span and backend span should share the same `trace-id`
- Backend span should show as a child of frontend span

### 3. Test Custom Spans
```bash
# Trigger the weather forecast endpoint
curl http://localhost:5100/weatherforecast

# Check Aspire Dashboard for:
# - GenerateWeatherForecast span
# - FetchWeatherData span (child)
# - Tags: forecast.days, forecast.unit, data.source
# - Event: ForecastGenerated
```

### 4. Test Error Handling
```csharp
// Temporarily add to Program.cs to test exception tracing
app.MapGet("/error", () =>
{
    using var activity = Telemetry.ActivitySource.StartActivity("TestError");
    throw new InvalidOperationException("Test exception for tracing");
});
```

Check Aspire Dashboard for:
- Span status: Error
- Event: "exception" with stacktrace

---

## 🔧 Configuration Reference

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `OTEL_EXPORTER_OTLP_ENDPOINT` | Backend OTLP endpoint (API) | None (uses Aspire defaults) |
| `VITE_OTEL_EXPORTER_OTLP_ENDPOINT` | Frontend OTLP endpoint (React) | `http://localhost:4318/v1/traces` |
| `ASPNETCORE_ENVIRONMENT` | Controls sampling strategy | `Development` |

### Sampling Configuration

**Development** (`Extensions.cs:71-74`):
```csharp
tracing.SetSampler(new AlwaysOnSampler()); // 100% sampling
```

**Production** (`Extensions.cs:76-80`):
```csharp
tracing.SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(0.1))); // 10%
```

**Change Production Sample Rate**:
```csharp
new TraceIdRatioBasedSampler(0.25) // 25% sampling
```

### Resource Attributes

Configured in `ServiceDefaults/Extensions.cs:46-56`:
```csharp
.ConfigureResource(resource => resource
    .AddService(serviceName, serviceVersion, serviceInstanceId)
    .AddAttributes(new[]
    {
        new KeyValuePair<string, object>("deployment.environment", environmentName),
        new KeyValuePair<string, object>("service.namespace", "aspire-react"),
    }))
```

---

## 🚨 Troubleshooting

### Issue: Frontend traces don't appear in Aspire Dashboard

**Symptoms**: Browser sends spans but they don't show in dashboard

**Solutions**:
1. Check CORS on OTLP endpoint:
   ```typescript
   // telemetry.ts - Add CORS headers if needed
   headers: { 'Access-Control-Allow-Origin': '*' }
   ```

2. Verify OTLP endpoint is reachable:
   ```bash
   curl -X POST http://localhost:4318/v1/traces \
     -H "Content-Type: application/json" \
     -d '{"resourceSpans":[]}'
   # Should return 200 OK
   ```

3. Check browser console for export errors:
   ```
   Failed to send spans: NetworkError
   ```

### Issue: Frontend and backend traces are disconnected

**Symptoms**: Two separate traces instead of one parent-child tree

**Solutions**:
1. Verify `traceparent` header is present:
   ```typescript
   // Check in telemetry.ts:58-62
   propagateTraceHeaderCorsUrls: [/localhost:\d+\/api/]
   ```

2. Ensure API CORS allows `traceparent` header:
   ```csharp
   // Program.cs:9-14
   policy.AllowAnyHeader(); // Must include traceparent
   ```

### Issue: Custom spans not appearing

**Symptoms**: ASP.NET Core spans visible, but custom spans missing

**Solutions**:
1. Verify ActivitySource name matches registration:
   ```csharp
   // Telemetry.cs
   public const string ActivitySourceName = "AspireReact.Api";

   // Extensions.cs:65
   tracing.AddSource(builder.Environment.ApplicationName) // Must match
   ```

2. Check activity is not null:
   ```csharp
   var activity = Telemetry.ActivitySource.StartActivity("MySpan");
   if (activity == null) // Span was dropped by sampler
       return;
   ```

### Issue: High cardinality causing performance issues

**Symptoms**: Telemetry backend slow, high memory usage

**Solutions**:
1. Avoid unique IDs as tags:
   ```csharp
   // ❌ BAD: Creates a new tag combination for every request
   activity?.SetTag("request.id", Guid.NewGuid().ToString());

   // ✅ GOOD: Use baggage for correlation IDs
   Activity.Current?.AddBaggage("request.id", requestId);
   ```

2. Increase sampling rate:
   ```csharp
   new TraceIdRatioBasedSampler(0.01) // 1% sampling
   ```

### Issue: Spans have incorrect duration

**Symptoms**: Span duration is 0ms or extremely long

**Solutions**:
1. Use `using` statements:
   ```csharp
   // ✅ GOOD: Automatically disposes when leaving scope
   using var activity = Telemetry.ActivitySource.StartActivity("MySpan");

   // ❌ BAD: Must manually call activity.Dispose()
   var activity = Telemetry.ActivitySource.StartActivity("MySpan");
   // ... forgot to dispose, span never closes
   ```

2. Avoid async void:
   ```csharp
   // ❌ BAD: using disposes immediately, before async work completes
   using var activity = ...;
   _ = Task.Run(async () => await SlowOperationAsync()); // Runs after dispose

   // ✅ GOOD: Wait for async work
   using var activity = ...;
   await SlowOperationAsync();
   ```

---

## 🏭 Production Considerations

### 1. OTLP Endpoint Configuration

**Development**: Aspire Dashboard (http://localhost:4318)
**Production**: Use a collector (e.g., OpenTelemetry Collector, Jaeger, Tempo)

```bash
# Set environment variable for backend
export OTEL_EXPORTER_OTLP_ENDPOINT=https://otel-collector.company.com

# Set in frontend .env.production
VITE_OTEL_EXPORTER_OTLP_ENDPOINT=https://otel-collector.company.com/v1/traces
```

### 2. Restrict CORS for Frontend Spans

```typescript
// telemetry.ts - Production configuration
propagateTraceHeaderCorsUrls: [
    /^https:\/\/api\.company\.com\/.*$/, // Only propagate to your API
],
```

### 3. Add Authentication to OTLP Endpoint

```typescript
// telemetry.ts
const otlpExporter = new OTLPTraceExporter({
    url: import.meta.env.VITE_OTEL_EXPORTER_OTLP_ENDPOINT,
    headers: {
        'Authorization': `Bearer ${import.meta.env.VITE_OTEL_AUTH_TOKEN}`
    },
});
```

### 4. Monitor Telemetry Overhead

- **Sampling**: Start with 10%, adjust based on volume
- **Batch Size**: Increase `BatchSpanProcessor` limits for high throughput
  ```csharp
  provider.AddSpanProcessor(new BatchSpanProcessor(otlpExporter, new()
  {
      MaxQueueSize = 10240,
      MaxExportBatchSize = 512,
  }));
  ```

### 5. PII and Compliance

**Never log**:
- ❌ Passwords, tokens, API keys
- ❌ Credit card numbers, SSNs
- ❌ User emails (unless anonymized)

**Redact sensitive data**:
```csharp
activity?.SetTag("user.id", HashUserId(userId)); // Hash instead of raw ID
activity?.SetTag("query", RedactSql(sqlQuery));  // Remove WHERE clause values
```

### 6. Resource Limits

**Recommended per trace**:
- Max spans: ~100 (more = expensive queries)
- Max attributes per span: ~50
- Max baggage items: ~5
- Total baggage size: <1KB

---

## 📚 Additional Resources

### OpenTelemetry Documentation
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)
- [OpenTelemetry JavaScript](https://opentelemetry.io/docs/languages/js/)
- [Semantic Conventions](https://opentelemetry.io/docs/specs/semconv/)
- [W3C Trace Context](https://www.w3.org/TR/trace-context/)

### .NET Aspire
- [.NET Aspire Overview](https://learn.microsoft.com/dotnet/aspire/get-started/aspire-overview)
- [Service Defaults](https://learn.microsoft.com/dotnet/aspire/fundamentals/service-defaults)
- [Dashboard](https://learn.microsoft.com/dotnet/aspire/fundamentals/dashboard)

### Best Practices
- [OpenTelemetry Best Practices](https://opentelemetry.io/docs/concepts/instrumentation/manual/)
- [Distributed Tracing Patterns](https://www.cncf.io/blog/2022/05/05/distributed-tracing-patterns/)
- [High Cardinality Pitfalls](https://opentelemetry.io/docs/specs/otel/metrics/data-model/#cardinality-limits)

---

## 🤝 Contributing

Contributions welcome! Please ensure:
1. All new instrumentation includes XML documentation
2. Custom spans follow OpenTelemetry semantic conventions
3. No PII in span attributes or events
4. MSTest + FluentAssertions for testing (follow .NET Best Practices)

### Development Setup

See **[DEVELOPMENT.md](./DEVELOPMENT.md)** for:
- Development environment setup
- Markdown linting with Marksman
- VS Code recommended extensions
- Code quality checks before committing
- Troubleshooting guide

---

## 📄 License

This project is provided as-is for educational and demonstration purposes.

---

## 🔖 Key Takeaways

1. **End-to-End Tracing Works**: W3C Trace Context headers connect browser and API spans
2. **ServiceDefaults Pattern**: Centralize observability config in a shared project
3. **Custom Spans Add Value**: Instrument business logic, not just framework code
4. **Sampling Reduces Cost**: 10% production sampling captures 100% of issues statistically
5. **Resource Attributes Matter**: Service identification enables filtering/aggregation
6. **Baggage ≠ Tags**: Use baggage for correlation, tags for filtering
7. **Aspire Simplifies Ops**: No manual collector config in development

**Next Steps**:
- Add metrics (counters, histograms) for business KPIs
- Implement log correlation with trace IDs
- Set up alerting on error rates from telemetry data
- Explore exemplars (linking metrics to traces)

---

## ⚖️ .NET Aspire: Pros & Cons

### Traditional Approach vs .NET Aspire

*Side-by-side comparison - Traditional multi-service setup vs Aspire orchestration*

```
┌─────────────────────────────────────────────────────────────────────────┐
│                    Traditional Multi-Service Setup                      │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  Developer Machine                                                      │
│  ┌───────────────────────────────────────────────────────────────────┐ │
│  │                                                                     │ │
│  │  ┌─────────────────┐    ┌──────────────────┐                      │ │
│  │  │ docker-compose  │    │ Multiple terminal│                      │ │
│  │  │     .yml        │    │ windows open     │                      │ │
│  │  └────────┬────────┘    └──────────────────┘                      │ │
│  │           │                                                         │ │
│  │           ▼                                                         │ │
│  │  ┌──────────────┐   ┌──────────────┐   ┌──────────────┐          │ │
│  │  │ API Service  │   │ Frontend     │   │ PostgreSQL   │          │ │
│  │  │ port: 5000   │   │ port: 3000   │   │ port: 5432   │          │ │
│  │  └──────┬───────┘   └──────┬───────┘   └──────┬───────┘          │ │
│  │         │                   │                   │                  │ │
│  │         └───────────────────┴───────────────────┘                  │ │
│  │                             │                                       │ │
│  │         ❌ Manual OpenTelemetry setup for each service             │ │
│  │         ❌ Manual retry/circuit breaker policies                   │ │
│  │         ❌ Manual logging configuration                            │ │
│  │         ❌ Manual health check endpoints                           │ │
│  │         ❌ Hardcoded service URLs                                  │ │
│  │         ❌ Separate logging/tracing dashboards                     │ │
│  │         ❌ Environment variables scattered across files            │ │
│  │                                                                     │ │
│  └─────────────────────────────────────────────────────────────────────┘ │
│                                                                         │
│  Configuration Files Needed:                                           │
│  📄 docker-compose.yml                                                 │
│  📄 appsettings.json (per service)                                     │
│  📄 .env files (per service)                                           │
│  📄 Custom logging setup                                               │
│  📄 Custom OTel configuration                                          │
│  📄 Polly resilience policies                                          │
│  📄 Service discovery config                                           │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

                              ⬇️ Migrate to Aspire ⬇️

┌─────────────────────────────────────────────────────────────────────────┐
│                    .NET Aspire Setup (This Project)                     │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  Developer Machine                                                      │
│  ┌───────────────────────────────────────────────────────────────────┐ │
│  │                                                                     │ │
│  │  ┌─────────────────────────────────────────────┐                  │ │
│  │  │  AppHost.cs (C# Orchestration)              │                  │ │
│  │  │  ────────────────────────────────            │                  │ │
│  │  │  var api = builder                          │                  │ │
│  │  │    .AddProject<Projects.Api>("api");        │                  │ │
│  │  │                                              │                  │ │
│  │  │  builder.AddViteApp("frontend", ...)        │                  │ │
│  │  │    .WithReference(api)     // ✅ Discovery  │                  │ │
│  │  │    .WaitFor(api);          // ✅ Order      │                  │ │
│  │  └────────┬────────────────────────────────────┘                  │ │
│  │           │  dotnet run (one command!)                             │ │
│  │           ▼                                                         │ │
│  │  ┌──────────────┐   ┌──────────────┐   ┌──────────────┐          │ │
│  │  │ API Service  │◀──│ Frontend     │   │ PostgreSQL   │          │ │
│  │  │ auto port    │   │ auto port    │   │ auto port    │          │ │
│  │  └──────┬───────┘   └──────┬───────┘   └──────┬───────┘          │ │
│  │         │                   │                   │                  │ │
│  │         └───────────────────┴───────────────────┘                  │ │
│  │                             │                                       │ │
│  │                             ▼                                       │ │
│  │                   ┌─────────────────────┐                          │ │
│  │                   │ Aspire Dashboard    │                          │ │
│  │                   ├─────────────────────┤                          │ │
│  │                   │ ✅ Traces           │                          │ │
│  │                   │ ✅ Logs             │                          │ │
│  │                   │ ✅ Metrics          │                          │ │
│  │                   │ ✅ Resources        │                          │ │
│  │                   │ ✅ Console Output   │                          │ │
│  │                   └─────────────────────┘                          │ │
│  │                                                                     │ │
│  │         ✅ Auto OpenTelemetry (ServiceDefaults)                    │ │
│  │         ✅ Built-in resilience (one line of code)                  │ │
│  │         ✅ Auto logging configuration                              │ │
│  │         ✅ Auto health checks (/health, /alive)                    │ │
│  │         ✅ Service discovery (no hardcoded URLs)                   │ │
│  │         ✅ Unified dashboard                                       │ │
│  │         ✅ Environment-based sampling (dev vs prod)                │ │
│  │                                                                     │ │
│  └─────────────────────────────────────────────────────────────────────┘ │
│                                                                         │
│  Configuration Files Needed:                                           │
│  📄 AppHost.cs (single orchestration file)                             │
│  📄 ServiceDefaults (shared across all services)                       │
│  ✅ That's it! (90% less config)                                       │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

---

### ✅ Pros of Using .NET Aspire

#### 1. **Developer Productivity**
```
Traditional:           Aspire:
────────────           ────────
⏱️  30 min setup       ⏱️  5 min setup
📝 500 lines config    📝 50 lines config
🐛 Debug across 5 tools 🐛 One dashboard
```

- **One-Command Start**: `dotnet run` in AppHost starts everything
- **Automatic Service Discovery**: Services find each other - no hardcoded URLs
- **Built-in Dashboard**: View logs, traces, metrics in one place
- **Hot Reload**: Changes reflect immediately without restarting containers

#### 2. **Observability Out-of-the-Box**
```
┌─────────────────────────────────────────────────────────┐
│  ServiceDefaults provides:                              │
├─────────────────────────────────────────────────────────┤
│  ✅ OpenTelemetry tracing (AspNetCore, HttpClient)     │
│  ✅ Structured logging                                  │
│  ✅ Metrics collection                                  │
│  ✅ Health checks                                       │
│  ✅ Resource attributes (service name, version, env)   │
│  ✅ Environment-based sampling (dev=100%, prod=10%)    │
└─────────────────────────────────────────────────────────┘
```

*Financial impact - 90% reduction in telemetry costs with environment-based sampling*

**Cost Impact**: 90% reduction in telemetry costs with environment-based sampling

#### 3. **Production-Ready Patterns**
```csharp
// Traditional: Manual Polly configuration (~50 lines)
services.AddHttpClient("api")
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy())
    .AddPolicyHandler(GetTimeoutPolicy());

// Aspire: One line
http.AddStandardResilienceHandler();  // ← Retry + Circuit Breaker + Timeout
```

Built-in resilience includes:
- **Retry**: 3 attempts with exponential backoff
- **Circuit Breaker**: Opens after 5 failures, half-open after 30s
- **Timeout**: 30 seconds default

#### 4. **First-Class Framework Support**
- **Vite/React**: `AddViteApp()` with auto environment injection
- **Next.js**: `AddNextApp()` with automatic routing
- **Node.js**: `AddNodeApp()` for Express, Fastify, etc.
- **Python**: `AddPythonProject()` for FastAPI, Flask

#### 5. **Deployment Flexibility**
```
Development:    Production:
────────────    ───────────
Local (DCP)  →  Kubernetes
             →  Azure Container Apps
             →  Docker Compose
             →  Any container orchestrator
```

Aspire generates deployment manifests for production environments.

---

### ❌ Cons of Using .NET Aspire

#### 1. **Learning Curve**
```
Prerequisite Knowledge:
┌────────────────────────────────┐
│ ✅ .NET/C# development         │
│ ✅ Basic Docker concepts       │
│ ✅ HTTP/REST APIs              │
│ ⚠️  Aspire-specific patterns   │ ← New concepts to learn
│ ⚠️  OpenTelemetry concepts     │
└────────────────────────────────┘
```

- **New Abstractions**: `AppHost`, `ServiceDefaults`, `.WaitFor()`, etc.
- **Team Onboarding**: Developers need training on Aspire concepts
- **Debugging**: Errors can be harder to debug if you don't understand the abstraction

#### 2. **Framework Lock-In**
```
┌────────────────────────────────────────────────┐
│  Dependency on Microsoft's Aspire Ecosystem    │
├────────────────────────────────────────────────┤
│  ⚠️  Tied to .NET version updates              │
│  ⚠️  Breaking changes in major versions        │
│  ⚠️  Migration effort if leaving ecosystem     │
└────────────────────────────────────────────────┘
```

**Example**: This project uses Aspire 13 which requires .NET 10+ (see migration ADR below)

#### 3. **Opinionated Patterns**
```
Aspire's Way:               Your Way:
─────────────               ─────────
✅ ServiceDefaults pattern   ❌ Custom OTel setup
✅ AppHost orchestration     ❌ Custom docker-compose
✅ Standard resilience       ❌ Fine-tuned Polly policies
```

- **Less Flexibility**: Aspire enforces specific patterns
- **Custom Configurations**: Harder to implement non-standard setups
- **Workarounds**: May need hacks for edge cases

#### 4. **Production Deployment Complexity**
```
Development:               Production:
────────────               ───────────
DCP (simple)     vs       Kubernetes (complex)
                          ↓
                  Aspire generates manifests but:
                  ⚠️  Requires understanding of K8s
                  ⚠️  Manual tweaks often needed
                  ⚠️  CI/CD pipeline integration
```

Aspire simplifies **development**, but production deployment still requires ops knowledge.

#### 5. **Resource Overhead (Development)**
```
┌──────────────────────────────────────────┐
│  What Aspire Runs Locally:               │
├──────────────────────────────────────────┤
│  📦 Docker Desktop (required)            │
│  📦 DCP (Aspire orchestrator)            │
│  📦 Aspire Dashboard (Blazor app)        │
│  📦 Your services (API, frontend, etc.)  │
└──────────────────────────────────────────┘

RAM Usage: ~2-4 GB for Aspire infrastructure alone
```

**Trade-off**: More resource usage for better developer experience

#### 6. **Rapid Version Changes**
```
Version Timeline:
──────────────────
9.5 (Nov 2024)  →  13.0 (Dec 2024)  →  ???
   ↑                    ↑
   6 weeks between major versions!

Impact:
⚠️  Frequent breaking changes
⚠️  Migration effort every few months
⚠️  Docs may lag behind
```

Aspire is evolving rapidly - expect migration work (see [ADR](#-architecture-decision-records) below).

---

### 🎯 When to Use .NET Aspire

```
┌─────────────────────────────────────────────────────────────────────┐
│                    Decision Matrix                                  │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ✅ Use Aspire If:                                                  │
│  ─────────────────────────────────────────────────────────          │
│  • Building a new .NET cloud-native app                            │
│  • Multiple services (API, frontend, database, cache, etc.)        │
│  • Team comfortable with .NET ecosystem                            │
│  • Need observability (tracing, logging, metrics)                  │
│  • Want rapid prototyping with production-ready patterns           │
│  • Deploying to Azure (first-class support)                        │
│                                                                     │
│  ❌ Avoid Aspire If:                                                │
│  ─────────────────────────────────────────────────────────          │
│  • Building a monolithic app (single service)                      │
│  • Using non-.NET backend (Python, Go, Java, etc.)                 │
│  • Need highly customized infrastructure setup                     │
│  • Team unfamiliar with .NET or containers                         │
│  • Low-resource development environment                            │
│  • Need stable APIs (Aspire is evolving rapidly)                   │
│                                                                     │
│  🤔 Consider Alternatives:                                          │
│  ─────────────────────────────────────────────────────────          │
│  • Docker Compose (more control, broader ecosystem)                │
│  • Kubernetes + Skaffold (production-grade, language-agnostic)     │
│  • Tye (simpler, less opinionated predecessor to Aspire)           │
│  • Manual setup (maximum flexibility, maximum effort)              │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

### 📊 Comparison Summary

| Aspect | Traditional Approach | .NET Aspire (This Project) |
|--------|---------------------|---------------------------|
| **Setup Time** | 30-60 minutes | 5 minutes |
| **Lines of Config** | 500+ (docker-compose, OTel, Polly, etc.) | 50 (AppHost + ServiceDefaults) |
| **Observability** | Manual setup per service | Automatic (ServiceDefaults) |
| **Resilience** | Manual Polly policies (~50 lines) | One line (`AddStandardResilienceHandler()`) |
| **Service Discovery** | Hardcoded URLs or DNS | Automatic |
| **Dashboard** | Separate tools (Jaeger, Grafana, etc.) | Built-in Aspire Dashboard |
| **Frontend Support** | Manual npm scripts | First-class `AddViteApp()` |
| **Learning Curve** | Steep (Docker, OTel, K8s) | Moderate (Aspire patterns) |
| **Flexibility** | High (full control) | Medium (opinionated) |
| **Lock-In** | None | .NET ecosystem |
| **Production Maturity** | Battle-tested | Evolving (rapid changes) |
| **Cost (Telemetry)** | High (100% sampling) | Low (environment-based: 10% prod) |

---

## 📋 Architecture Decision Records

For detailed analysis of .NET Aspire versions, migration decisions, and trade-offs, see:

### 📄 [ADR: Aspire 9.5 → 13 Migration](./ADR_ASPIRE_MIGRATION_9_TO_13.md)
**Topics covered**:
- Cost-benefit analysis (90% telemetry cost reduction)
- Migration complexity assessment
- Breaking changes and mitigation strategies
- PostgreSQL integration considerations
- Risk assessment and rollback plan

**Key Findings**:
- **ROI**: 2-5 day migration pays for itself in < 1 week of production savings
- **Performance**: 40% faster dashboard, 2x faster container startup
- **Recommendation**: Migrate to Aspire 13 (benefits outweigh costs)

### 📄 [Aspire Version Comparison](./ASPIRE_VERSION_COMPARISON.md)
**Visual guide showing**:
- Architecture diagrams (9.5 vs 13)
- Request flow comparisons
- OpenTelemetry configuration differences
- Frontend integration (AddNpmApp vs AddViteApp)
- Feature matrix
- Performance benchmarks

**Use this document** to understand what changed between Aspire versions and how this project implements modern patterns.

---

### Why This Project Uses Aspire 13

This repository uses **.NET Aspire 13.0.2** for the following reasons:

1. **Environment-Based Sampling**: 90% cost reduction in production (10% sampling vs 100%)
2. **Built-in Resilience**: One line vs 50 lines of Polly configuration
3. **Vite Support**: First-class React integration with `AddViteApp()`
4. **Service Dependencies**: `.WaitFor()` ensures proper startup order
5. **HTTP Transport**: Solves certificate issues in local development
6. **Future-Proof**: Aligns with .NET 10+ roadmap

**Migration Status**: ✅ Successfully migrated from Aspire 9.5 (see ADR)

---

## 🏁 Final Verdict

**.NET Aspire is excellent for**:
- ✅ New .NET cloud-native projects
- ✅ Teams building distributed systems
- ✅ Projects requiring observability
- ✅ Rapid prototyping with production patterns

**But consider alternatives if**:
- ❌ You're not using .NET
- ❌ You need maximum flexibility
- ❌ Your team is unfamiliar with .NET
- ❌ You're building a simple monolith

**This project demonstrates** that Aspire delivers on its promise: **production-ready observability with minimal configuration**. The trade-off is accepting Microsoft's opinionated patterns and keeping up with rapid version changes.

For migration decisions and detailed analysis, refer to the [ADR documentation](#-architecture-decision-records) above
