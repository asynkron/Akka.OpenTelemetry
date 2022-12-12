using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Configuration;
using Akka.OpenTelemetry;
using Akka.OpenTelemetry.Local;
using Akka.OpenTelemetry.Telemetry;

namespace Akka;

public static class ActorOfUtils
{
    public static bool NotTraced(Props props, bool systemService, ActorPath path)
    {
        return systemService || path.Elements.FirstOrDefault() != "user";
    }

    public static IInternalActorRef LocalActorOf(ActorSystemImpl system, Props props, IInternalActorRef supervisor,
        ActorPath path, Deploy deploy, bool lookupDeploy, bool async)
    {
        var props2 = props;
        var propsDeploy = lookupDeploy ? system.Provider.Deployer.Lookup(path) : deploy;
        if (propsDeploy != null)
        {
            if (propsDeploy.Mailbox != Deploy.NoMailboxGiven)
                props2 = props2.WithMailbox(propsDeploy.Mailbox);
            if (propsDeploy.Dispatcher != Deploy.NoDispatcherGiven)
                props2 = props2.WithDispatcher(propsDeploy.Dispatcher);
        }

        if (!system.Dispatchers.HasDispatcher(props2.Dispatcher))
            throw new ConfigurationException($"Dispatcher [{props2.Dispatcher}] not configured for path {path}");

        try
        {
            // for consistency we check configuration of dispatcher and mailbox locally
            var dispatcher = system.Dispatchers.Lookup(props.Dispatcher);
            var mailboxType = system.Mailboxes.GetMailboxType(props, dispatcher.Configurator.Config);

            var settings = new OpenTelemetrySettings(true);
            IInternalActorRef reff = async switch
            {
                true => new OpenTelemetryRepointableActorRef(settings, system, props2, dispatcher, mailboxType,
                    supervisor,
                    path).Initialize(async),
                _ => new OpenTelemetryLocalActorRef(system, props, dispatcher, mailboxType, supervisor, path)
            };

            system.Hooks().ActorSpawned(settings, props, reff);

            return reff;
        }
        catch (Exception ex)
        {
            throw new ConfigurationException(
                $"Configuration problem while creating [{path}] with dispatcher [{props.Dispatcher}] and mailbox [{props.Mailbox}]",
                ex);
        }
    }
}