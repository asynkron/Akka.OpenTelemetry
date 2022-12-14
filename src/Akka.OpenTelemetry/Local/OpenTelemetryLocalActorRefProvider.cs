using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Event;
using JetBrains.Annotations;

namespace Akka.OpenTelemetry.Local;

[UsedImplicitly]
public sealed class OpenTelemetryLocalActorRefProvider : LocalActorRefProviderDecorator
{
    public OpenTelemetryLocalActorRefProvider(string systemName, Settings settings, EventStream eventStream)
    {
        Inner = new LocalActorRefProvider(systemName, settings, eventStream);
    }

    public override void Init(ActorSystemImpl system)
    {
        Inner.Init(system);
    }

    public override IInternalActorRef ActorOf(ActorSystemImpl system, Props props, IInternalActorRef supervisor,
        ActorPath path,
        bool systemService, Deploy deploy, bool lookupDeploy, bool async)
    {
        if (ActorOfUtils.NotTraced(systemService, path))
            return base.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);

        //reuse the spawn logic
        return ActorOfUtils.LocalActorOf(system, props, supervisor, path, deploy, lookupDeploy, async);
    }

    // public override FutureActorRef<T> CreateFutureRef<T>(TaskCompletionSource<T> tcs)
    // {
    //     var path = TempPath();
    //     var future = new FutureActorRef<T>(tcs, path, this);
    //     return future;
    // }
}