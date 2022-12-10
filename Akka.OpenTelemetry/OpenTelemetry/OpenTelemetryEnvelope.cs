using Akka.Actor;

namespace Akka.OpenTelemetry;

public record OpenTelemetryEnvelope(object Message, Dictionary<string, string> Headers) : INotInfluenceReceiveTimeout;