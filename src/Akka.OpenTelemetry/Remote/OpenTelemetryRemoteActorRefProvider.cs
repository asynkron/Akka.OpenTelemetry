using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Event;
using Akka.Remote;
using JetBrains.Annotations;

namespace Akka.OpenTelemetry.Remote;

[UsedImplicitly]
public sealed class OpenTelemetryRemoteActorRefProvider : IRemoteActorRefProviderProxy, IRemoteActorRefProvider
{
    public OpenTelemetryRemoteActorRefProvider(string systemName, Settings settings, EventStream eventStream) : base(
        new RemoteActorRefProvider2(systemName, settings, eventStream))
    {

    }

    public new IInternalActorRef ActorOf(ActorSystemImpl system, Props props, IInternalActorRef supervisor,
        ActorPath path,
        bool systemService, Deploy deploy, bool lookupDeploy, bool async)
    {
        if (ActorOfUtils.NotTraced(props, systemService, path))
            return base.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);

        //TODO: figure out what to do with remote deployments here...

        //reuse the spawn logic
        return ActorOfUtils.LocalActorOf(system, props, supervisor, path, deploy, lookupDeploy, async);
    }
}