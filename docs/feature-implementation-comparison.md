# Feature Implementation Comparison: Aspire 9.5 vs Aspire 13

> **Side-by-side code examples** showing how to implement the same features in both Aspire versions

---

## 📋 Table of Contents

1. [Project Setup](#1-project-setup)
2. [Adding a React/Vite Frontend](#2-adding-a-reactvite-frontend)
3. [Adding a PostgreSQL Database](#3-adding-a-postgresql-database)
4. [Configuring Resilience Policies](#4-configuring-resilience-policies)
5. [Service Dependencies](#5-service-dependencies)
6. [OpenTelemetry Sampling](#6-opentelemetry-sampling)
7. [OTLP Transport Configuration](#7-otlp-transport-configuration)
8. [Health Checks](#8-health-checks)
9. [Container Publishing](#9-container-publishing)
10. [Service Discovery](#10-service-discovery)

---

## 1. Project Setup

### Aspire 9.5 (.NET 9)

```xml
<!-- AppHost.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <OutputType>Exe</OutputType>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="9.5.0" />
  </ItemGroup>
</Project>
```

```csharp
// Program.cs
var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.MyApi>("api");

builder.Build().Run();
```

**Package Versions**:
- `Aspire.Hosting.AppHost`: 9.5.0
- `Aspire.ServiceDefaults`: 9.5.0

---

### Aspire 13 (.NET 10+)

```xml
<!-- AppHost.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <OutputType>Exe</OutputType>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="13.0.2" />
  </ItemGroup>
</Project>
```

```csharp
// Program.cs
var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.MyApi>("api")
    .WithExternalHttpEndpoints();  // ← NEW: Expose endpoints externally

builder.Build().Run();
```

**Package Versions**:
- `Aspire.Hosting.AppHost`: 13.0.2
- `Aspire.ServiceDefaults`: 13.0.2

**Key Differences**:
- ✅ Aspire 13: `WithExternalHttpEndpoints()` for external access
- ✅ Aspire 13: Better endpoint management

---

## 2. Adding a React/Vite Frontend

### Aspire 9.5 (.NET 9)

```csharp
// AppHost.cs
var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.MyApi>("api");

// ❌ Manual npm configuration
var frontend = builder.AddNpmApp("frontend", "../MyApp.React")
    .WithReference(api)
    .WithEnvironment("VITE_API_URL", api.GetEndpoint("http"))
    .WithHttpEndpoint(port: 3000, env: "PORT");

// ⚠️  No service dependencies - frontend may start before API
// ⚠️  Manual environment variable injection
// ⚠️  Generic npm handling (not Vite-specific)

builder.Build().Run();
```

**Limitations**:
- No `.WaitFor()` - race conditions on startup
- Manual environment variables
- No automatic containerization

---

### Aspire 13 (.NET 10+)

```csharp
// AppHost.cs
var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.MyApi>("api")
    .WithExternalHttpEndpoints();

// ✅ First-class Vite support
var frontend = builder.AddViteApp("frontend", "../MyApp.React")
    .WithReference(api)              // ← Service discovery (automatic)
    .WaitFor(api)                    // ← NEW: Frontend waits for API
    .WithEnvironment("BROWSER", "none")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();          // ← NEW: Auto-containerization

builder.Build().Run();
```

**Improvements**:
- ✅ `.WaitFor(api)` ensures correct startup order
- ✅ `PublishAsDockerFile()` auto-generates Dockerfile
- ✅ Vite-specific optimizations
- ✅ Automatic environment variable injection

**Startup Sequence**:
```
Aspire 9.5:                 Aspire 13:
───────────                 ──────────
1. API starts               1. API starts
2. Frontend starts          2. API health check passes
   (may fail if API slow)   3. Frontend starts ✅
                            4. Frontend connects successfully
```

---

## 3. Adding a PostgreSQL Database

### Aspire 9.5 (.NET 9)

```csharp
// AppHost.cs
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()                // Persist data
    .AddDatabase("mydb");

var api = builder.AddProject<Projects.MyApi>("api")
    .WithReference(postgres);

// ⚠️  Basic setup only
// ⚠️  No built-in health checks
// ⚠️  No pgAdmin support
// ⚠️  No init script mounting
```

**API Connection** (`appsettings.json`):
```json
{
  "ConnectionStrings": {
    "postgres": "Host=localhost;Database=mydb;Username=postgres;Password=..."
  }
}
```

---

### Aspire 13 (.NET 10+)

```csharp
// AppHost.cs
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin()                   // ← NEW: Built-in pgAdmin UI
    .WithHealthCheck()               // ← NEW: Automatic health checks
    .AddDatabase("mydb")
    .WithInitBindMount("./init");    // ← NEW: Mount init SQL scripts

var api = builder.AddProject<Projects.MyApi>("api")
    .WithReference(postgres)
    .WaitFor(postgres);              // ← NEW: Wait for DB to be ready

// Access pgAdmin at: http://localhost:5050
```

**Improvements**:
- ✅ `WithPgAdmin()` - web-based database management
- ✅ `WithHealthCheck()` - verify database readiness
- ✅ `WithInitBindMount()` - run seed scripts automatically
- ✅ `.WaitFor(postgres)` - API waits for DB

**Health Check Example**:
```
Aspire Dashboard → Resources → postgres
Status: ✅ Healthy (responds to ping)
```

---

## 4. Configuring Resilience Policies

### Aspire 9.5 (.NET 9)

```csharp
// ServiceDefaults/Extensions.cs
public static IHostApplicationBuilder AddServiceDefaults(
    this IHostApplicationBuilder builder)
{
    // ❌ MANUAL Polly configuration required
    builder.Services.AddHttpClient("default")
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy())
        .AddPolicyHandler(GetTimeoutPolicy());

    return builder;
}

// ~50 lines of boilerplate
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryCount, context) =>
            {
                Log.Warning("Retry {RetryCount} after {Delay}ms",
                    retryCount, timespan.TotalMilliseconds);
            });
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30),
            onBreak: (outcome, duration) =>
            {
                Log.Error("Circuit breaker opened for {Duration}s",
                    duration.TotalSeconds);
            },
            onReset: () => Log.Information("Circuit breaker reset"));
}

static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
{
    return Policy.TimeoutAsync<HttpResponseMessage>(
        TimeSpan.FromSeconds(10));
}
```

**Total Lines**: ~50 lines of Polly configuration

---

### Aspire 13 (.NET 10+)

```csharp
// ServiceDefaults/Extensions.cs
public static IHostApplicationBuilder AddServiceDefaults(
    this IHostApplicationBuilder builder)
{
    // ✅ ONE LINE = Retry + Circuit Breaker + Timeout
    builder.Services.ConfigureHttpClientDefaults(http =>
    {
        http.AddStandardResilienceHandler();  // ← 🎉 That's it!
        http.AddServiceDiscovery();
    });

    return builder;
}
```

**Total Lines**: 1 line!

**What `AddStandardResilienceHandler()` includes**:
```
┌────────────────────────────────────────┐
│ Standard Resilience Handler            │
├────────────────────────────────────────┤
│ ✅ Retry: 3 attempts, exponential      │
│    backoff (2s, 4s, 8s)                │
│ ✅ Circuit Breaker: Open after 5       │
│    failures, half-open after 30s       │
│ ✅ Timeout: 30 seconds                 │
│ ✅ Bulkhead: Limit concurrent requests │
│ ✅ Rate Limiting: Prevent overload     │
└────────────────────────────────────────┘
```

**Customization** (if needed):
```csharp
http.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 5;
    options.CircuitBreaker.FailureRatio = 0.5;
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(60);
});
```

**Comparison**:
```
Aspire 9.5: ~50 lines of manual Polly code
Aspire 13:   1 line with sensible defaults ✅
```

---

## 5. Service Dependencies

### Aspire 9.5 (.NET 9)

```csharp
// AppHost.cs
var postgres = builder.AddPostgres("postgres")
    .AddDatabase("mydb");

var api = builder.AddProject<Projects.MyApi>("api")
    .WithReference(postgres);

var frontend = builder.AddNpmApp("frontend", "../MyApp.React")
    .WithReference(api);

// ❌ NO DEPENDENCY ORDERING
// Services start in parallel - race conditions possible!
```

**Startup Issues**:
```
Time: 0s ──────────> 5s ──────────> 10s
        │              │               │
        ▼              ▼               ▼
  All start     API connects    Frontend loads
  together      to postgres     (may fail if API
                (may fail if    not ready)
                DB not ready)
```

**Workarounds** (manual):
- Add retry logic in application code
- Use health checks with delays
- Custom startup scripts

---

### Aspire 13 (.NET 10+)

```csharp
// AppHost.cs
var postgres = builder.AddPostgres("postgres")
    .WithHealthCheck()
    .AddDatabase("mydb");

var api = builder.AddProject<Projects.MyApi>("api")
    .WithReference(postgres)
    .WaitFor(postgres);              // ← NEW: Wait for DB

var frontend = builder.AddViteApp("frontend", "../MyApp.React")
    .WithReference(api)
    .WaitFor(api);                   // ← NEW: Wait for API

// ✅ GUARANTEED STARTUP ORDER
```

**Startup Sequence**:
```
Time: 0s ─────> 3s ─────> 6s ─────> 9s
        │         │         │         │
        ▼         ▼         ▼         ▼
    postgres   API waits  API      Frontend
    starts     for DB     starts   waits for API
               health
               check              Frontend
                                  starts
```

**Benefits**:
- ✅ No race conditions
- ✅ Clean error messages (if dependency fails)
- ✅ Faster debugging

---

## 6. OpenTelemetry Sampling

### Aspire 9.5 (.NET 9)

```csharp
// ServiceDefaults/Extensions.cs
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddSource(builder.Environment.ApplicationName);

        // ❌ STATIC SAMPLING (same everywhere)
        tracing.SetSampler(new AlwaysOnSampler());  // 100% sampling
    });

// Problem: 100% sampling in production = $$$
```

**Cost Impact**:
```
10,000 requests/day × 5 services × 5 spans = 250,000 spans/day
× 30 days = 7,500,000 spans/month
× $0.02 per 1000 spans = $1,500/month
```

---

### Aspire 13 (.NET 10+)

```csharp
// ServiceDefaults/Extensions.cs
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddSource(builder.Environment.ApplicationName);

        // ✅ ENVIRONMENT-BASED SAMPLING
        if (builder.Environment.IsDevelopment())
        {
            // Development: 100% sampling (debug everything)
            tracing.SetSampler(new AlwaysOnSampler());
        }
        else
        {
            // Production: 10% sampling (90% cost reduction)
            tracing.SetSampler(new ParentBasedSampler(
                new TraceIdRatioBasedSampler(0.1)));  // 10%
        }
    });
```

**Cost Impact** (Production):
```
10,000 requests/day × 5 services × 5 spans × 0.1 = 25,000 spans/day
× 30 days = 750,000 spans/month
× $0.02 per 1000 spans = $150/month

SAVINGS: $1,500 - $150 = $1,350/month (90% reduction) ✅
```

**Sampling Strategies**:
```
Environment    Aspire 9.5    Aspire 13    Savings
───────────    ──────────    ─────────    ───────
Development    100%          100%         0%
Staging        100%          50%          50%
Production     100%          10%          90% ✅
```

---

## 7. OTLP Transport Configuration

### Aspire 9.5 (.NET 9)

```json
// launchSettings.json
{
  "profiles": {
    "https": {
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL": "https://localhost:19178"
      }
    }
  }
}
```

**Problem**:
```
❌ HTTPS REQUIRED - No workaround
❌ Certificate must be trusted
❌ Common error: UntrustedRoot exception

Fix (manual):
$ dotnet dev-certs https --trust
$ # Restart everything and pray 🙏
```

**Error Message** (cryptic):
```
System.Net.Http.HttpRequestException:
The SSL connection could not be established,
see inner exception.
---> System.Security.Authentication.AuthenticationException:
The remote certificate is invalid according to the validation procedure.
```

---

### Aspire 13 (.NET 10+)

```json
// launchSettings.json
{
  "profiles": {
    "https": {
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL": "http://localhost:19178",
        "ASPIRE_ALLOW_UNSECURED_TRANSPORT": "true"  // ← NEW: HTTP OK!
      }
    }
  }
}
```

**Benefits**:
```
✅ HTTP allowed for localhost (with explicit flag)
✅ Clear error message if flag missing:
   "ASPIRE_ALLOW_UNSECURED_TRANSPORT must be set to 'true'
    to use HTTP for OTLP endpoints"
✅ Works out of the box
✅ Production can still use HTTPS
```

**Architecture**:
```
┌──────────────────────────────────────┐
│ User Browser ──HTTPS──> Dashboard   │  (External: HTTPS)
│                                      │
│ Dashboard ──HTTP──> OTLP Endpoint   │  (Internal: HTTP OK)
│              (localhost only)        │
└──────────────────────────────────────┘
```

---

## 8. Health Checks

### Aspire 9.5 (.NET 9)

```csharp
// ServiceDefaults/Extensions.cs
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());

// Program.cs (in each service)
app.MapHealthChecks("/health");  // Manual endpoint

// ⚠️  Basic health check only
// ⚠️  No liveness/readiness separation
// ⚠️  Not Kubernetes-ready
```

**Endpoints**:
- `/health` - generic health check

---

### Aspire 13 (.NET 10+)

```csharp
// ServiceDefaults/Extensions.cs
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]);

// Program.cs
app.MapDefaultEndpoints();  // ← NEW: Auto-configures /health and /alive

// ✅ Two endpoints automatically configured:
//    /health - Readiness (is service ready to handle requests?)
//    /alive  - Liveness (is service running?)
```

**Kubernetes Integration**:
```yaml
# deployment.yaml (auto-generated by Aspire)
livenessProbe:
  httpGet:
    path: /alive
    port: 8080
  initialDelaySeconds: 3
  periodSeconds: 10

readinessProbe:
  httpGet:
    path: /health
    port: 8080
  initialDelaySeconds: 5
  periodSeconds: 10
```

**Benefits**:
- ✅ Kubernetes-ready out of the box
- ✅ Separate liveness/readiness probes
- ✅ Tagged health checks for filtering

---

## 9. Container Publishing

### Aspire 9.5 (.NET 9)

```csharp
// AppHost.cs
var frontend = builder.AddNpmApp("frontend", "../MyApp.React");

// ❌ No automatic containerization
// ❌ Must manually create Dockerfile
```

**Manual Dockerfile** (required):
```dockerfile
# Dockerfile (in MyApp.React/)
FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

**Deployment**:
```bash
# Manual steps
$ cd MyApp.React
$ docker build -t myapp-frontend .
$ docker push myapp-frontend
$ # Update Kubernetes manifests
```

---

### Aspire 13 (.NET 10+)

```csharp
// AppHost.cs
var frontend = builder.AddViteApp("frontend", "../MyApp.React")
    .PublishAsDockerFile();  // ← NEW: Auto-generates Dockerfile

// ✅ Dockerfile created automatically
// ✅ Optimized multi-stage build
// ✅ Production-ready nginx configuration
```

**Auto-Generated Dockerfile**:
```dockerfile
# Auto-generated by Aspire ✅
FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci --only=production
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 8080
HEALTHCHECK --interval=30s CMD wget -q --spider http://localhost:8080 || exit 1
CMD ["nginx", "-g", "daemon off;"]
```

**Deployment**:
```bash
# Aspire generates manifests automatically
$ dotnet publish
$ # Dockerfile, K8s manifests, and deployment scripts auto-generated
```

---

## 10. Service Discovery

### Aspire 9.5 (.NET 9)

```csharp
// AppHost.cs
var api = builder.AddProject<Projects.MyApi>("api");

var frontend = builder.AddNpmApp("frontend", "../MyApp.React")
    .WithEnvironment("VITE_API_URL", api.GetEndpoint("http"));
    // ⚠️  Hardcoded URL resolution

// Frontend code
const API_URL = import.meta.env.VITE_API_URL;  // "http://localhost:5100"
fetch(`${API_URL}/weatherforecast`);
```

**Limitations**:
- Manual environment variable management
- Hardcoded URLs in environment files
- Breaks if ports change

---

### Aspire 13 (.NET 10+)

```csharp
// AppHost.cs
var api = builder.AddProject<Projects.MyApi>("api")
    .WithExternalHttpEndpoints();

var frontend = builder.AddViteApp("frontend", "../MyApp.React")
    .WithReference(api);  // ← Automatic service discovery

// ✅ Environment variables injected automatically:
//    services__api__http__0=http://localhost:5100
//    services__api__https__0=https://localhost:5101
```

**Frontend code**:
```typescript
// No hardcoded URLs! Use service discovery
const apiUrl = import.meta.env.VITE_services__api__http__0 || 'http://localhost:5100';
fetch(`${apiUrl}/weatherforecast`);

// Or use the Aspire SDK (recommended):
import { getServiceUrl } from '@aspire/client';
const apiUrl = await getServiceUrl('api');
```

**Benefits**:
- ✅ No hardcoded URLs
- ✅ Works in dev, staging, prod (Kubernetes DNS)
- ✅ Automatic port management

---

## 📊 Summary Comparison Table

| Feature | Aspire 9.5 | Aspire 13 | Improvement |
|---------|-----------|-----------|-------------|
| **Frontend Setup** | Manual `AddNpmApp` | `AddViteApp` with auto-config | First-class support |
| **Service Dependencies** | ❌ No ordering | `WaitFor()` guaranteed order | Eliminates race conditions |
| **Resilience** | ~50 lines Polly | 1 line `AddStandardResilienceHandler()` | 98% less code |
| **Sampling** | Static 100% | Environment-based (10% prod) | 90% cost savings |
| **OTLP Transport** | HTTPS only | HTTP allowed (dev) | Easier local dev |
| **Health Checks** | Manual `/health` | Auto `/health` + `/alive` | Kubernetes-ready |
| **Container Publishing** | Manual Dockerfile | `PublishAsDockerFile()` | Auto-generated |
| **Service Discovery** | Manual env vars | Automatic injection | Zero configuration |
| **Database Tooling** | Basic setup | `WithPgAdmin()`, health checks | Better DX |

---

## 🎯 Key Takeaways

**Aspire 9.5**:
- Solid foundation
- Requires manual configuration
- More boilerplate code
- Production-ready but verbose

**Aspire 13**:
- Modern, streamlined API
- Built-in best practices
- Significantly less code
- Production-ready out of the box

**Migration ROI**:
```
Code Reduction: ~70% less boilerplate
Cost Savings: ~90% in telemetry costs
Performance: 40% faster dashboard, 2x faster startup
Developer Experience: ⭐⭐⭐⭐⭐
```

---

**See also**:
- [ADR: Migration Analysis](../ADR_ASPIRE_MIGRATION_9_TO_13.md)
- [Version Comparison](../ASPIRE_VERSION_COMPARISON.md)
- [Decision Tree](./decision-tree.md)

---

**Last Updated**: 2025-12-17
**Aspire Versions**: 9.5.0 vs 13.0.2
