namespace Akka.OpenTelemetry;

public record OpenTelemetrySettings(bool EnableTracing, string? ParentId);