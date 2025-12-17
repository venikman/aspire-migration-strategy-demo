import { WebTracerProvider } from '@opentelemetry/sdk-trace-web';
import { Resource } from '@opentelemetry/resources';
import { ATTR_SERVICE_NAME, ATTR_SERVICE_VERSION } from '@opentelemetry/semantic-conventions';
import { registerInstrumentations } from '@opentelemetry/instrumentation';
import { FetchInstrumentation } from '@opentelemetry/instrumentation-fetch';
import { DocumentLoadInstrumentation } from '@opentelemetry/instrumentation-document-load';
import { UserInteractionInstrumentation } from '@opentelemetry/instrumentation-user-interaction';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-http';
import { BatchSpanProcessor } from '@opentelemetry/sdk-trace-web';
import { ZoneContextManager } from '@opentelemetry/context-zone';

/**
 * Initializes OpenTelemetry instrumentation for the React frontend.
 * This enables automatic tracing for fetch requests, page loads, and user interactions.
 *
 * Trace context is propagated to the backend via W3C Trace Context headers,
 * enabling end-to-end distributed tracing from browser to API.
 */
export function initializeTelemetry(): void {
  // Configure resource attributes to identify this service in telemetry backends
  const resource = Resource.default().merge(
    new Resource({
      [ATTR_SERVICE_NAME]: 'aspire-react-frontend',
      [ATTR_SERVICE_VERSION]: '1.0.0',
      'deployment.environment': import.meta.env.MODE, // 'development' or 'production'
    })
  );

  // Create the tracer provider with resource metadata
  const provider = new WebTracerProvider({
    resource: resource,
  });

  // Configure OTLP exporter to send traces to the backend
  // In Aspire, this will be routed through the AppHost to the dashboard
  const otlpExporter = new OTLPTraceExporter({
    url: import.meta.env.VITE_OTEL_EXPORTER_OTLP_ENDPOINT || 'http://localhost:4318/v1/traces',
    headers: {},
  });

  // Use BatchSpanProcessor for better performance (batches spans before export)
  provider.addSpanProcessor(new BatchSpanProcessor(otlpExporter));

  // Register the provider globally
  provider.register({
    contextManager: new ZoneContextManager(), // Zone.js context propagation for async operations
  });

  // Register automatic instrumentations
  registerInstrumentations({
    instrumentations: [
      // Automatically traces all fetch() calls
      new FetchInstrumentation({
        // Propagate trace context to backend via W3C Trace Context headers
        propagateTraceHeaderCorsUrls: [
          /localhost:\d+\/api/, // Match /api/* endpoints
          /.*/, // Allow all origins for now - restrict in production
        ],
        // Clear timing resources after processing to prevent memory leaks
        clearTimingResources: true,
        // Add custom attributes to fetch spans
        applyCustomAttributesOnSpan: (span, request, result) => {
          if (request instanceof Request) {
            span.setAttribute('http.request.url', request.url);
          }
          if (result instanceof Response) {
            span.setAttribute('http.response.status_code', result.status);
            span.setAttribute('http.response.status_text', result.statusText);
          }
        },
      }),
      // Traces document load events (DOMContentLoaded, load, etc.)
      new DocumentLoadInstrumentation(),
      // Traces user interactions (clicks, etc.)
      new UserInteractionInstrumentation({
        eventNames: ['click', 'submit'], // Only track meaningful interactions
      }),
    ],
  });

  console.log('✅ OpenTelemetry initialized for frontend');
}
