using System.Diagnostics;
using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Dispatch;

namespace Akka.OpenTelemetry.ActorRefs;

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
        var actorCell = new OpenTelemetryActorCell(_settings, System, this, Props, Dispatcher, Supervisor);
        actorCell.Init(false, _mailboxType);
        return actorCell;
    }

    protected override void TellInternal(object message, IActorRef sender)
    {
        var actorRefTag = Activity.Current?.GetTagItem(OtelTags.ActorRef)?.ToString() ?? "none";

        using var activity = OpenTelemetryHelpers.BuildStartedActivity(Activity.Current.Context, actorRefTag, "Tell", message,
            OpenTelemetryHelpers.DefaultSetupActivity);

        //TODO: probably have to exclude a lot of control messages here?
        var headers = Activity.Current?.Context.GetPropagationHeaders();
        var envelope = new OpenTelemetryEnvelope(message, headers ?? Headers.Empty);
        base.TellInternal(envelope, sender);
    }
}