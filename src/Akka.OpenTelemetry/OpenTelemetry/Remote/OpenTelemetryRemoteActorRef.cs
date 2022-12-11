using Akka.Actor;
using Akka.Remote;

namespace Akka.OpenTelemetry.Remote;

public class OpenTelemetryRemoteActorRef : RemoteActorRef, ICanTell
{
    public new void Tell(object message, IActorRef sender)
    {
        var envelope = TraceTell.ExtractHeaders(message);
        base.Tell(envelope,sender);
    }

    public OpenTelemetryRemoteActorRef(RemoteTransport remote, Address localAddressToUse, ActorPath path, IInternalActorRef parent, Props props, Deploy deploy) : base(remote, localAddressToUse, path, parent, props, deploy)
    {
    }
}