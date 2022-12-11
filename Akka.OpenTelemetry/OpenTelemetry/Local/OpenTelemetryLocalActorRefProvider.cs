using System.Diagnostics;
using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Configuration;
using Akka.Decorators;
using Akka.Event;
using Akka.OpenTelemetry.Local.ActorRefs;
using JetBrains.Annotations;

namespace Akka.OpenTelemetry.Local;

[UsedImplicitly]
public sealed class OpenTelemetryLocalActorRefProvider : DecoratorActorRefProvider<LocalActorRefProvider>
{
    private ActorSystemImpl _system = null!;

    public OpenTelemetryLocalActorRefProvider(string systemName, Settings settings, EventStream eventStream)
    {
        SetInner(new LocalActorRefProvider(systemName, settings, eventStream));
    }

    public override void Init(ActorSystemImpl system)
    {
        _system = system;
        Inner.Init(system);
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
            return base.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);
        }

        //Random copy paste from LocalActorRefProvider
        var props2 = props;
        var propsDeploy = lookupDeploy ? Deployer.Lookup(path) : deploy;
        if (propsDeploy != null)
        {
            if (propsDeploy.Mailbox != Deploy.NoMailboxGiven)
                props2 = props2.WithMailbox(propsDeploy.Mailbox);
            if (propsDeploy.Dispatcher != Deploy.NoDispatcherGiven)
                props2 = props2.WithDispatcher(propsDeploy.Dispatcher);
        }

        if (!system.Dispatchers.HasDispatcher(props2.Dispatcher))
        {
            throw new ConfigurationException($"Dispatcher [{props2.Dispatcher}] not configured for path {path}");
        }

        try
        {
            // for consistency we check configuration of dispatcher and mailbox locally
            var dispatcher = _system.Dispatchers.Lookup(props.Dispatcher);
            var mailboxType = _system.Mailboxes.GetMailboxType(props, dispatcher.Configurator.Config);

            var settings = new OpenTelemetrySettings(true);
            return async switch
            {
                true => new OpenTelemetryRepointableActorRef(settings, system, props2, dispatcher, mailboxType, supervisor, path).Initialize(async),
                _ => new OpenTelemetryLocalActorRef(settings, system, props, dispatcher, mailboxType, supervisor, path)
            };
        }
        catch (Exception ex)
        {
            throw new ConfigurationException(
                $"Configuration problem while creating [{path}] with dispatcher [{props.Dispatcher}] and mailbox [{props.Mailbox}]",
                ex);
        }
    }
}