var builder = DistributedApplication.CreateBuilder(args);

// C# Minimal API backend
var api = builder.AddProject<Projects.AspireReact_Api>("api");

// React frontend with npm (Aspire 9.5 - no AddViteApp yet)
// ⚠️  No .WaitFor() in Aspire 9.5 - race conditions possible
// ⚠️  No .PublishAsDockerFile() - manual Dockerfile required
builder.AddNpmApp("frontend", "../AspireReact.React")
    .WithReference(api)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(port: 3000, env: "PORT");

builder.Build().Run();
