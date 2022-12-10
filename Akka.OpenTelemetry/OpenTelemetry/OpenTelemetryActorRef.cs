using System.Diagnostics;
using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Dispatch;
using Akka.Dispatch.SysMsg;

namespace Akka.OpenTelemetry;

public class OpenTelemetryActorRef : LocalActorRef
{
    public OpenTelemetryActorRef(ActorSystemImpl system, Props props, MessageDispatcher dispatcher, MailboxType mailboxType, IInternalActorRef supervisor, ActorPath path) : base(system, props, dispatcher, mailboxType, supervisor, path)
    {
    }

    protected override void TellInternal(object message, IActorRef sender)
    {
        if (message is ISystemMessage)
        {
            base.TellInternal(message,sender);
            return;
        }


        var actorRefTag = Activity.Current?.GetTagItem(OtelTags.ActorRef)?.ToString() ?? "none";

        using var activity = OpenTelemetryHelpers.BuildStartedActivity(Activity.Current.Context, actorRefTag, "Tell", message,
            OpenTelemetryHelpers.DefaultSetupActivity);

        //TODO: probably have to exclude a lot of control messages here?
        var headers = Activity.Current?.Context.GetPropagationHeaders();
        var envelope = new OpenTelemetryEnvelope(message, headers ?? Headers.Empty);
        base.TellInternal(envelope, sender);
    }

    protected override ActorCell NewActorCell(ActorSystemImpl system, IInternalActorRef self, Props props, MessageDispatcher dispatcher,
        IInternalActorRef supervisor)
    {
        return new OpenTelemetryActorCell(system, self, props, dispatcher, supervisor);
    }
}