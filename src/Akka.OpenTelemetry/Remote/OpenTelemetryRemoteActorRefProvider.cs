using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Event;
using JetBrains.Annotations;

namespace Akka.OpenTelemetry.Remote;

[UsedImplicitly]
public sealed class OpenTelemetryRemoteActorRefProvider: RemoteActorRefProviderDecorator
{
    public OpenTelemetryRemoteActorRefProvider(string systemName, Settings settings, EventStream eventStream)
    {

        Inner = new RemoteActorRefProvider2(systemName, settings, eventStream);
    }

    public override void Init(ActorSystemImpl system)
    {
        Inner.Init(system);
        Logging.GetLogger(system.EventStream, "OpenTelemetryRemoteActorRefProvider(" + Inner.RootPath.Address + ")");
    }

    public override IInternalActorRef ActorOf(ActorSystemImpl system, Props props, IInternalActorRef supervisor,
        ActorPath path,
        bool systemService, Deploy deploy, bool lookupDeploy, bool async)
    {
        if (ActorOfUtils.NotTraced(props, systemService, path))
        {
            Console.WriteLine("Not traced " + path);
            return base.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);
        }

        //TODO: figure out what to do with remote deployments here...

        //reuse the spawn logic
        return ActorOfUtils.LocalActorOf(system, props, supervisor, path, deploy, lookupDeploy, async);
    }
}