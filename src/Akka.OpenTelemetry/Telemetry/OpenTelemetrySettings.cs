using Akka.OpenTelemetry.Cell;

namespace Akka.OpenTelemetry.Telemetry;

public record OpenTelemetrySettings(bool EnableTracing)
{
    //mutable
    public string? ParentSpanId { get; set; }
    public OpenTelemetryActorCell Context { get; set; } = null!;
}