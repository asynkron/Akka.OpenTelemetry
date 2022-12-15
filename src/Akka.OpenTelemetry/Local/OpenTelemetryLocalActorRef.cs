using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Dispatch;
using Akka.OpenTelemetry.Cell;
using Akka.OpenTelemetry.Telemetry;

namespace Akka.OpenTelemetry.Local;

public class OpenTelemetryLocalActorRef : LocalActorRef
{
    public OpenTelemetryLocalActorRef(ActorSystemImpl system, Props props,
        MessageDispatcher dispatcher, MailboxType mailboxType, IInternalActorRef supervisor, ActorPath path) : base(
        system, props, dispatcher, mailboxType, supervisor, path)
    {
    }

    protected override void TellInternal(object message, IActorRef sender)
    {
        var envelope = OpenTelemetryHelpers.ExtractHeaders(message, Props);
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

    protected override ActorCell NewActorCell(ActorSystemImpl system, IInternalActorRef self, Props props,
        MessageDispatcher dispatcher,
        IInternalActorRef supervisor)
    {
        system.Hooks().ActorRefNewCell(this, system, self, props, dispatcher, supervisor);
        return new OpenTelemetryActorCell(system, self, props, dispatcher, supervisor);
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