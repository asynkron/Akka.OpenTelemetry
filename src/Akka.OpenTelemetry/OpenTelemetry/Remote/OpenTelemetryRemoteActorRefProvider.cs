using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Decorators;
using Akka.Event;
using Akka.Remote;
using JetBrains.Annotations;

namespace Akka.OpenTelemetry.Remote;

[UsedImplicitly]
public sealed class OpenTelemetryRemoteActorRefProvider: RemoteActorRefProviderDecorator
{
    private ILoggingAdapter _log;
    private ActorSystemImpl _system;
    public OpenTelemetryRemoteActorRefProvider(string systemName, Settings settings, EventStream eventStream)
    {

        _inner = new RemoteActorRefProvider2(systemName, settings, eventStream);
    }

    public override void Init(ActorSystemImpl system)
    {
        _system = system;
        _inner.Init(system);
        _log = Logging.GetLogger(system.EventStream, "OpenTelemetryRemoteActorRefProvider(" + Inner.RootPath.Address + ")");
    }

    public override IInternalActorRef ActorOf(ActorSystemImpl system, Props props, IInternalActorRef supervisor,
        ActorPath path,
        bool systemService, Deploy deploy, bool lookupDeploy, bool async)
    {
        if (Spawner.NotTraced(props, systemService, path))
        {
            Console.WriteLine("Not traced " + path);
            return base.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);
        }

        //TODO: figure out what to do with remote deployments here...

        //reuse the spawn logic
        return Spawner.LocalActorOf(system, props, supervisor, path, deploy, lookupDeploy, async);
    }

    public override IActorRef ResolveActorRef(string path)
    {
        if (ActorPath.TryParse(path, out var actorPath) && actorPath.Address == Inner.RootPath.Address)
            return ResolveActorRef((RootGuardianActorRef)Inner.RootGuardian, actorPath.Elements);

        _log.Debug("Resolve of unknown path [{0}] failed. Invalid format.", path);
        return Inner.DeadLetters;
    }

    public override IActorRef ResolveActorRef(ActorPath actorPath)
    {
        return ResolveActorRef((RootGuardianActorRef)Inner.RootGuardian, actorPath.Elements);
    }

    private IActorRef ResolveActorRef(IInternalActorRef actorRef, IReadOnlyList<string> pathElements)
    {
        if (pathElements.Count == 0)
        {
            _log.Debug("Resolve of empty path sequence fails (per definition)");
            return Inner.DeadLetters;
        }
        var child = actorRef.GetChild(pathElements);
        if (child.IsNobody())
        {
            _log.Debug("Resolve of path sequence [/{0}] failed", ActorPath.FormatPathElements(pathElements));
            return new EmptyLocalActorRef(_system.Provider, actorRef.Path / pathElements, _system.EventStream);
        }
        return (IInternalActorRef)child;
    }
}