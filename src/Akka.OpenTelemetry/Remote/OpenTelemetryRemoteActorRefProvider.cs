using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Event;
using Akka.Remote;
using JetBrains.Annotations;

namespace Akka.OpenTelemetry.Remote;

[UsedImplicitly]
public sealed class OpenTelemetryRemoteActorRefProvider : RemoteActorRefProvider, IRemoteActorRefProvider
{
    public OpenTelemetryRemoteActorRefProvider(string systemName, Settings settings, EventStream eventStream) : base(systemName, settings, eventStream)
    {
    }

    protected override IInternalActorRef CreateRemoteRef(Props props, IInternalActorRef supervisor,
        Address localAddress, ActorPath rpath,
        Deploy deployment)
    {
        if (ActorOfUtils.NotTraced(false, rpath))
            return new RemoteActorRef(Transport, localAddress, rpath, supervisor, props, deployment);

        return new OpenTelemetryRemoteActorRef(Transport, localAddress, rpath, supervisor, props, deployment);
    }

    protected override IInternalActorRef CreateRemoteRef(ActorPath actorPath, Address localAddress)
    {
        //if the remote is a temp. don't trace it
        if (ActorOfUtils.NotTraced(false, actorPath))
        {
            return new RemoteActorRef(Transport, localAddress, actorPath, ActorRefs.Nobody, Props.None,
                Deploy.None);
        }

        return new OpenTelemetryRemoteActorRef(Transport, localAddress, actorPath, ActorRefs.Nobody, Props.None,
            Deploy.None);
    }

    public new IInternalActorRef ActorOf(ActorSystemImpl system, Props props, IInternalActorRef supervisor,
        ActorPath path,
        bool systemService, Deploy deploy, bool lookupDeploy, bool async)
    {
        if (ActorOfUtils.NotTraced(systemService, path))
            return base.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);

        //TODO: figure out what to do with remote deployments here...

        //reuse the spawn logic
        return ActorOfUtils.LocalActorOf(system, props, supervisor, path, deploy, lookupDeploy, async);
    }

    public new FutureActorRef<T> CreateFutureRef<T>(TaskCompletionSource<T> tcs)
    {
        var path = TempPath();
        var future = new FutureActorRef<T>(tcs, path, this);
        return future;
    }

}