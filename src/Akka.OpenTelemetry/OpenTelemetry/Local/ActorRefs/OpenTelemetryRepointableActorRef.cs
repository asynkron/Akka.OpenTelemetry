using System.Diagnostics;
using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Dispatch;
using Akka.OpenTelemetry.Cell;

namespace Akka.OpenTelemetry.Local.ActorRefs;

public class OpenTelemetryRepointableActorRef : RepointableActorRef
{
    private readonly OpenTelemetrySettings _settings;
    private readonly MailboxType _mailboxType;

    public OpenTelemetryRepointableActorRef(OpenTelemetrySettings settings, ActorSystemImpl system, Props props, MessageDispatcher dispatcher, MailboxType mailboxType, IInternalActorRef supervisor, ActorPath path) : base(system, props, dispatcher, mailboxType, supervisor, path)
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
        var envelope = TraceTell.ExtractHeaders(message, Props);
        base.TellInternal(envelope, sender);
    }
}