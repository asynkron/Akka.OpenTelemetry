using System.Diagnostics;
using System.Runtime.CompilerServices;
using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Dispatch;
using Akka.Dispatch.SysMsg;

namespace Akka.OpenTelemetry.Cell;

public class OpenTelemetryActorCell : ActorCell
{
    private readonly OpenTelemetrySettings _openTelemetrySettings;
    private string _parentSpanId;

    public OpenTelemetryActorCell(OpenTelemetrySettings openTelemetrySettings,  ActorSystemImpl system, IInternalActorRef self, Props props,
        MessageDispatcher dispatcher, IInternalActorRef parent) : base(system, self, props, dispatcher, parent)
    {
        _openTelemetrySettings = new OpenTelemetrySettings(true, "");
        _parentSpanId = null;

        // if (openTelemetrySettings == null)
        // {
        //
        // }
        // else
        // {
        //     _openTelemetrySettings = openTelemetrySettings;
        //     _parentSpanId = _openTelemetrySettings.ParentId ?? "";
        // }
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
            if (Activity.Current != null)
            {
                _parentSpanId = Activity.Current.Id!;
            }
            else
            {
                _parentSpanId = _openTelemetrySettings.ParentId!;
            }

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

            _parentSpanId = activity.Id;
            //shady, yes, but we need to trigger receive timeout etc.
            Invoke(new Envelope(envelope.Message, Sender, System));
        }
        else
        {
            Invoke(new Envelope(envelope.Message, Sender, System));
        }
    }

    void ReceiveActivitySetup(Activity? activity, object message)
    {
        activity?.SetTag(OtelTags.ActorType, Props.Type.Name)
            .SetTag(OtelTags.MessageType, message.GetTypeName())
            .SetTag(OtelTags.ActorRef, Self.ToString())
            .SetTag(OtelTags.SenderActorRef, Sender.ToString());
    }

    protected override void PreStart()
    {
        using var activity = OpenTelemetryHelpers.ActivitySource.StartActivity(nameof(PreStart), ActivityKind.Server, _parentSpanId);
        AddEvent(activity);
        base.PreStart();
    }

    public override void Start()
    {
        using var activity = OpenTelemetryHelpers.ActivitySource.StartActivity(nameof(Start), ActivityKind.Server, _parentSpanId);
        AddEvent(activity);
        base.Start();
    }

    // protected override ActorBase CreateNewActorInstance()
    // {
    //     using var activity = OpenTelemetryHelpers.ActivitySource.StartActivity(nameof(CreateNewActorInstance), ActivityKind.Server, _parentSpanId);
    //     AddEvent(activity);
    //     var res = base.CreateNewActorInstance();
    //     return res;
    // }

    private void AddEvent(Activity? activity,[CallerMemberName]string callerName ="")
    {
        activity?.AddEvent(new ActivityEvent(callerName));
        activity?.AddTag(OtelTags.ActorType, Props.Type.Name);
    }

    protected override void AutoReceiveMessage(Envelope envelope)
    {
        Console.WriteLine("AroundReceive");
        using var activity = OpenTelemetryHelpers.ActivitySource.StartActivity(nameof(AutoReceiveMessage), ActivityKind.Server, _parentSpanId);
        AddEvent(activity);
        base.AutoReceiveMessage(envelope);
    }

    // public override void SendMessage(Envelope message)
    // {
    //     using var activity = OpenTelemetryHelpers.ActivitySource.StartActivity(nameof(SendMessage), ActivityKind.Server, _parentSpanId);
    //     AddEvent(activity);
    //     activity?.AddEvent(new ActivityEvent("Message: " + message));
    //     base.SendMessage(message);
    // }
    //
    // public override void SendMessage(IActorRef sender, object message)
    // {
    //     using var activity = OpenTelemetryHelpers.ActivitySource.StartActivity(nameof(SendMessage), ActivityKind.Server, _parentSpanId);
    //     AddEvent(activity);
    //     activity?.AddEvent(new ActivityEvent("Message: " + message));
    //     base.SendMessage(sender, message);
    // }

    public override IActorRef ActorOf(Props props, string name = null)
    {
        using var activity = OpenTelemetryHelpers.ActivitySource.StartActivity(nameof(ActorOf), ActivityKind.Server, _parentSpanId);
        AddEvent(activity);
        activity?.AddEvent(new ActivityEvent("ActorOf: " + name));
        return base.ActorOf(props, name);
    }

    public override void SendSystemMessage(ISystemMessage systemMessage)
    {
        using var activity = OpenTelemetryHelpers.ActivitySource.StartActivity(nameof(SendSystemMessage), ActivityKind.Server, _parentSpanId);
        AddEvent(activity);
        activity?.AddEvent(new ActivityEvent("SystemMessage: " + systemMessage));
        base.SendSystemMessage(systemMessage);
    }
}