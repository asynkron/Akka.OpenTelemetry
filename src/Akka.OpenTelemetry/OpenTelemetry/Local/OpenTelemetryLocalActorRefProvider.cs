using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Decorators;
using Akka.Event;
using JetBrains.Annotations;

namespace Akka.OpenTelemetry.Local;

[UsedImplicitly]
public sealed class OpenTelemetryLocalActorRefProvider : ActorRefProviderDecorator
{
    public OpenTelemetryLocalActorRefProvider(string systemName, Settings settings, EventStream eventStream)
    {
        _localProvider = new LocalActorRefProvider(systemName, settings, eventStream);
    }

    public override void Init(ActorSystemImpl system)
    {
        LocalProvider.Init(system);
    }

    public override IInternalActorRef ActorOf(ActorSystemImpl system, Props props, IInternalActorRef supervisor,
        ActorPath path,
        bool systemService, Deploy deploy, bool lookupDeploy, bool async)
    {
        if (Spawner.NotTraced(props, systemService))
        {
            Console.WriteLine("Not traced " + path);
            return base.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);
        }

        //reuse the spawn logic
        return Spawner.LocalActorOf(system, props, supervisor, path, deploy, lookupDeploy, async);
    }
}