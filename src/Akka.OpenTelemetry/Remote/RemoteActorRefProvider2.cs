using Akka.Actor;
using Akka.Event;
using Akka.Remote;

namespace Akka.OpenTelemetry.Remote;

public class RemoteActorRefProvider2 : RemoteActorRefProvider
{
    public RemoteActorRefProvider2(string systemName, Settings settings, EventStream eventStream) : base(systemName, settings, eventStream)
    {
    }

    //Who TF thought it was a good idea to have some members virtual and others not?
    protected override IInternalActorRef CreateRemoteRef(Props props, IInternalActorRef supervisor, Address localAddress, ActorPath rpath,
        Deploy deployment)
    {
        return new OpenTelemetryRemoteActorRef(Transport, localAddress, rpath, supervisor, props, deployment);
    }

    protected override IInternalActorRef CreateRemoteRef(ActorPath actorPath, Address localAddress)
    {
        return new OpenTelemetryRemoteActorRef(Transport, localAddress, actorPath, ActorRefs.Nobody, Props.None, Deploy.None);

    }
}