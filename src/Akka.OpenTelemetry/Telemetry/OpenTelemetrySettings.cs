using Akka.Actor;

namespace Akka.OpenTelemetry.Telemetry;

public record OpenTelemetrySettings(bool EnableTracing)
{
    //mutable
    public string? ParentSpanId { get; set; }
}