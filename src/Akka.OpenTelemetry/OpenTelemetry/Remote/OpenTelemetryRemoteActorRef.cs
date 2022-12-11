using Akka.Actor;
using Akka.Remote;

namespace Akka.OpenTelemetry.Remote;

public class OpenTelemetryRemoteActorRef : RemoteActorRef, ICanTell
{
    public new void Tell(object message, IActorRef sender)
    {
        Console.WriteLine(message + "  -  " + sender);

        base.Tell(message,sender);
        // try
        // {
        //     var envelope = TraceTell.ExtractHeaders(message);
        //     var b = (RemoteActorRef)this;
        //     b.Tell(envelope, sender);
        // }
        // catch(Exception x)
        // {
        //     Console.WriteLine(x);
        // }
    }

    public OpenTelemetryRemoteActorRef(RemoteTransport remote, Address localAddressToUse, ActorPath path, IInternalActorRef parent, Props props, Deploy deploy) : base(remote, localAddressToUse, path, parent, props, deploy)
    {
    }
}