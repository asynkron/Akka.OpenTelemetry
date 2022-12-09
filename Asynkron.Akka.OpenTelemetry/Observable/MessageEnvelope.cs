namespace Asynkron.Akka.Observable;

public record MessageEnvelope(object Message, Dictionary<string, string> Headers);