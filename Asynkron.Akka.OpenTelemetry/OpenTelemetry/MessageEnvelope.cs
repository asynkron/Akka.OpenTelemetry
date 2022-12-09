namespace Asynkron.Akka.OpenTelemetry;

public record MessageEnvelope(object Message, Dictionary<string, string> Headers);