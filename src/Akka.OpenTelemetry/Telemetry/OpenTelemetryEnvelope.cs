using Akka.Actor;

namespace Akka.OpenTelemetry.Telemetry;

public static class Headers
{
    public static readonly Dictionary<string,string> Empty = new();

}

public record OpenTelemetryEnvelope(object Message, Dictionary<string, string> Headers) : INotInfluenceReceiveTimeout;