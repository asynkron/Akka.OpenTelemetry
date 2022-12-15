using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Dispatch;
using Akka.OpenTelemetry.Cell;
using Akka.OpenTelemetry.Telemetry;

namespace Akka.OpenTelemetry.Local;

public class OpenTelemetryRepointableActorRef : RepointableActorRef
{
    private readonly MailboxType _mailboxType;
    private readonly OpenTelemetrySettings _settings;

    public OpenTelemetryRepointableActorRef(OpenTelemetrySettings settings, ActorSystemImpl system, Props props,
        MessageDispatcher dispatcher, MailboxType mailboxType, IInternalActorRef supervisor, ActorPath path) : base(
        system, props, dispatcher, mailboxType, supervisor, path)
    {
        _settings = settings;
        _mailboxType = mailboxType;
    }

    protected override ActorCell NewCell()
    {
        var actorCell = new OpenTelemetryActorCell(System, this, Props, Dispatcher, Supervisor);
        actorCell.Init(false, _mailboxType);
        return actorCell;
    }

    protected override void TellInternal(object message, IActorRef sender)
    {
        var envelope = OpenTelemetryHelpers.ExtractHeaders(message, Props, sender);
        if (InternalCurrentActorCellKeeper.Current != null)
        {
            var system = InternalCurrentActorCellKeeper.Current.System;
            var self = InternalCurrentActorCellKeeper.Current.Self;
            var settings = (InternalCurrentActorCellKeeper.Current as OpenTelemetryActorCell)?.Settings;
            if (settings != null)
                //only call hook if we are in a tracable actor context
                system.Hooks().ActorRefTell(settings, message, self, this, sender);
        }

        base.TellInternal(envelope, sender);
    }

    public override void Restart(Exception cause)
    {
        System.Hooks().ActorRefRestart(this, cause);
        base.Restart(cause);
    }

    public override void Stop()
    {
        System.Hooks().ActorRefStop(this);
        base.Stop();
    }
}