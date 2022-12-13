using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Event;
using JetBrains.Annotations;

namespace Akka.OpenTelemetry.Local;

[UsedImplicitly]
public sealed class OpenTelemetryLocalActorRefProvider : LocalActorRefProviderProxy, IActorRefProvider
{
    public OpenTelemetryLocalActorRefProvider(string systemName, Settings settings, EventStream eventStream) : base(new LocalActorRefProvider(systemName, settings,eventStream))
    {

    }

    public new IInternalActorRef ActorOf(ActorSystemImpl system, Props props, IInternalActorRef supervisor,
        ActorPath path,
        bool systemService, Deploy deploy, bool lookupDeploy, bool async)
    {
        if (ActorOfUtils.NotTraced(props, systemService, path))
            return base.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);

        //reuse the spawn logic
        return ActorOfUtils.LocalActorOf(system, props, supervisor, path, deploy, lookupDeploy, async);
    }
}