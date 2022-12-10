using System.Diagnostics;
using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Dispatch;

namespace Akka.OpenTelemetry;

public class OpenTelemetryActorCell : ActorCell
{
    public OpenTelemetryActorCell(ActorSystemImpl system, IInternalActorRef self, Props props,
        MessageDispatcher dispatcher, IInternalActorRef parent) : base(system, self, props, dispatcher, parent)
    {
    }

    private OpenTelemetryEnvelope? _currentEnvelope;
    protected override void ReceiveMessage(object message)
    {
        //non augmented message, just pass it on
        if (message is not OpenTelemetryEnvelope envelope)
        {
            base.ReceiveMessage(message);
            return;
        }

        _currentEnvelope = envelope;

        var propagationContext = envelope.Headers.ExtractPropagationContext();

        var actorType = Actor.ToString();
        using var activity =
            OpenTelemetryHelpers.BuildStartedActivity(propagationContext.ActivityContext, actorType,
                nameof(ReceiveMessage),
                envelope.Message, ReceiveActivitySetup);

        base.ReceiveMessage(envelope.Message);
    }

    void ReceiveActivitySetup(Activity activity, object message)
    {
        activity?.SetTag(OtelTags.ActorType, Actor.ToString());
        activity?.SetTag(OtelTags.MessageType, message.GetTypeName());
        activity?.SetTag(OtelTags.ActorRef, Self.ToString());
        activity?.SetTag(OtelTags.SenderActorRef, Sender.ToString());
    }

}