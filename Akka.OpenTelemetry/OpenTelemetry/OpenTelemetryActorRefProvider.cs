using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Decorators;
using Akka.Event;

namespace Akka.OpenTelemetry;

public sealed class OpenTelemetryActorRefProvider : DecoratorActorRefProvider
{
    private ActorSystemImpl _system = null!;

    public OpenTelemetryActorRefProvider(string systemName, Settings settings, EventStream eventStream)
    {
        _inner = new LocalActorRefProvider(systemName, settings, eventStream);
        SetInner(_inner);
    }

    public override void Init(ActorSystemImpl system)
    {
        _system = system;
        _inner.Init(system);
    }

    public override IInternalActorRef ActorOf(ActorSystemImpl system, Props props, IInternalActorRef supervisor,
        ActorPath path,
        bool systemService, Deploy deploy, bool lookupDeploy, bool async)
    {
        if (systemService)
        {
            return base.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);
        }

        //vanilla stuff, just pass through to local actor ref provider
        if (props.Deploy is not OpenTelemetryDeploy otelDep)
        {
            var reff = base.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);
            return reff;
        }

        var dispatcher = _system.Dispatchers.Lookup(props.Dispatcher);
        var mailboxType = _system.Mailboxes.GetMailboxType(props, dispatcher.Configurator.Config);

        return new OpenTelemetryActorRef(system, props, dispatcher,
            mailboxType, supervisor, path);
    }
}