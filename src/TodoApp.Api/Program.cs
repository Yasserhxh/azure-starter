// Composition root for the TodoApp modular monolith host.
//
// Phase 1 is an intentionally minimal, buildable skeleton. Each phase plugs in
// its piece here through a single AddXxxModule()/MapXxxModule() pair:
//   - Phase 2: Todos module
//   - Phase 3: Identity module + BFF auth pipeline + antiforgery
//   - Phase 4: Serilog, correlation-id middleware, OpenTelemetry, ProblemDetails,
//              health checks (/health/live, /health/ready), metrics
//   - Phase 6: SignalR hub
// Keep this file a thin shell: wiring only, no business logic.

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    name = "TodoApp.Api",
    description = "Modular monolith host. See docs/architecture/overview.md.",
}));

app.Run();

// Exposed so TodoApp.Api.IntegrationTests can boot the host via WebApplicationFactory<Program>.
public partial class Program;
