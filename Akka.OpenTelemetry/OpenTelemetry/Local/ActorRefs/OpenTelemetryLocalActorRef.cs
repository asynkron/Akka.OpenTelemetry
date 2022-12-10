using System.Diagnostics;
using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Dispatch;

namespace Akka.OpenTelemetry.Local.ActorRefs;

public class OpenTelemetryLocalActorRef : LocalActorRef
{
    private readonly OpenTelemetrySettings _settings;

    public OpenTelemetryLocalActorRef(OpenTelemetrySettings settings, ActorSystemImpl system, Props props, MessageDispatcher dispatcher, MailboxType mailboxType, IInternalActorRef supervisor, ActorPath path) : base(system, props, dispatcher, mailboxType, supervisor, path)
    {
        _settings = settings;
    }

    protected override void TellInternal(object message, IActorRef sender)
    {
        var actorRefTag = Activity.Current?.GetTagItem(OtelTags.ActorRef)?.ToString() ?? "NoSender";

        using var activity = OpenTelemetryHelpers.BuildStartedActivity(Activity.Current.Context, actorRefTag, "Tell", message,
            OpenTelemetryHelpers.DefaultSetupActivity);
        Activity.Current.AddTag(OtelTags.ActorType, Props.Type.Name);

        //TODO: probably have to exclude a lot of control messages here?
        var headers = Activity.Current?.Context.GetPropagationHeaders();
        var envelope = new OpenTelemetryEnvelope(message, headers ?? Headers.Empty);
        base.TellInternal(envelope, sender);
    }

    protected override ActorCell NewActorCell(ActorSystemImpl system, IInternalActorRef self, Props props, MessageDispatcher dispatcher,
        IInternalActorRef supervisor)
    {
        return new OpenTelemetryActorCell(_settings, system, self, props, dispatcher, supervisor);
    }
}