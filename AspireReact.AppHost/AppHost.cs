var builder = DistributedApplication.CreateBuilder(args);

// C# Minimal API backend
var api = builder.AddProject<Projects.AspireReact_Api>("api")
    .WithExternalHttpEndpoints();

// React frontend with Vite
builder.AddViteApp("frontend", "../AspireReact.React")
    .WithReference(api)
    .WaitFor(api)
    .WithEnvironment("BROWSER", "none")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
