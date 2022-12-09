using Akka.Actor;
using Asynkron.Akka.Decorators;

namespace Asynkron.Akka.OpenTelemetry;

public class OpenTelemetryActorRef : DecoratorActorRef
{
    public OpenTelemetryActorRef(IInternalActorRef inner) : base(inner)
    {
    }

    public override void Tell(object message, IActorRef sender)
    {
        //TODO: capture activity context
        //bake into envelope message
        //copy from Proto.Actor

        var envelope = new MessageEnvelope(message, new Dictionary<string, string>());
        base.Tell(envelope, sender);
    }
}