using System.Diagnostics;
using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Dispatch;

namespace Akka.OpenTelemetry;

public class OpenTelemetryActorCell : ActorCell
{
    private readonly OpenTelemetrySettings _openTelemetrySettings;

    public OpenTelemetryActorCell(OpenTelemetrySettings openTelemetrySettings,  ActorSystemImpl system, IInternalActorRef self, Props props,
        MessageDispatcher dispatcher, IInternalActorRef parent) : base(system, self, props, dispatcher, parent)
    {
        _openTelemetrySettings = openTelemetrySettings;
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

        if (_openTelemetrySettings.EnableTracing)
        {
            var propagationContext = envelope.Headers.ExtractPropagationContext();

            var actorType = Actor.ToString();
            using var activity =
                OpenTelemetryHelpers
                    .BuildStartedActivity(
                        propagationContext.ActivityContext,
                        actorType ?? "null",
                        nameof(ReceiveMessage),
                        envelope.Message,
                        ReceiveActivitySetup);

            base.ReceiveMessage(envelope.Message);
        }
        else
        {
            base.ReceiveMessage(envelope.Message);
        }
    }

    void ReceiveActivitySetup(Activity? activity, object message)
    {
        activity?.SetTag(OtelTags.ActorType, Actor.ToString())
            .SetTag(OtelTags.MessageType, message.GetTypeName())
            .SetTag(OtelTags.ActorRef, Self.ToString())
            .SetTag(OtelTags.SenderActorRef, Sender.ToString());
    }
}