using System.Diagnostics;
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
        var headers = Activity.Current?.Context.GetPropagationHeaders();
        var envelope = new OpenTelemetryEnvelope(message, headers ?? Headers.Empty);
        base.Tell(envelope, sender);
    }
}