using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Configuration;
using Akka.Decorators;
using Akka.Event;
using Akka.OpenTelemetry.Local.ActorRefs;
using Akka.Remote;
using JetBrains.Annotations;

namespace Akka.OpenTelemetry.Remote;

[UsedImplicitly]
public sealed class OpenTelemetryRemoteActorRefProvider: RemoteActorRefProviderDecorator
{
    private ActorSystemImpl _system;

    public OpenTelemetryRemoteActorRefProvider(string systemName, Settings settings, EventStream eventStream)
    {
        _remoteProvider = new RemoteActorRefProvider(systemName, settings, eventStream);
    }

    public override void Init(ActorSystemImpl system)
    {
        _system = system;
        _remoteProvider.Init(system);
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

        //TODO: figure out what to do with remote deployments here...

        //reuse the spawn logic
        return Spawner.LocalActorOf(system, props, supervisor, path, deploy, lookupDeploy, async);
    }




    //Random copy paste from LocalActorRefProvider
}