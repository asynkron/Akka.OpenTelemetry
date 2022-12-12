using System.Diagnostics;
using System.Runtime.CompilerServices;
using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Dispatch;
using Akka.Dispatch.SysMsg;
using Akka.OpenTelemetry.Local;
using Akka.OpenTelemetry.Telemetry;

namespace Akka.OpenTelemetry.Cell;

public class OpenTelemetryActorCell : ActorCell, IActorRefFactory
{
    private readonly OpenTelemetrySettings _openTelemetrySettings;

    private OpenTelemetryEnvelope? _currentEnvelope;
    private string? _parentSpanId;

    public OpenTelemetryActorCell(ActorSystemImpl system, IInternalActorRef self, Props props,
        MessageDispatcher dispatcher, IInternalActorRef parent) : base(system, self, props, dispatcher, parent)
    {
        //TODO: where should this be injected?
        _openTelemetrySettings = new OpenTelemetrySettings(true);
        _parentSpanId = null;
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

    public override IActorRef ActorOf(Props props, string? name = null)
    {
        using var activity =
            OpenTelemetryHelpers.ActivitySource.StartActivity(nameof(ActorOf), ActivityKind.Server, _parentSpanId!);
        AddEvent(activity);
        var res = base.ActorOf(props, name);
        activity?.AddEvent(new ActivityEvent("Spawned Child: " + res));

        return res;
    }

    public new ActorSelection ActorSelection(string actorPath)
    {
        var selection = base.ActorSelection(actorPath);
        var tracableAnchor = new ActorSelectionAnchorActorRef((selection.Anchor as IInternalActorRef)!);
        var x = new ActorSelection(tracableAnchor, selection.Path);
        return x;
    }

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
            if (Activity.Current != null) _parentSpanId = Activity.Current.Id!;

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

            _parentSpanId = activity?.Id ?? "";
            //shady, yes, but we need to trigger receive timeout etc.
            ReceiveInner(envelope);
        }
        else
        {
            ReceiveInner(envelope);
        }
    }

    private void ReceiveInner(OpenTelemetryEnvelope envelope)
    {
        Invoke(new Envelope(envelope.Message, Sender, System));
        System.Hooks().ActorReceivedMessage(envelope.Message, Self);
    }

    private void ReceiveActivitySetup(Activity? activity, object message)
    {
        activity?.SetTag(OtelTags.ActorType, Props.Type.Name)
            .SetTag(OtelTags.MessageType, message.GetTypeName())
            .SetTag(OtelTags.ActorRef, Self.ToString())
            .SetTag(OtelTags.SenderActorRef, Sender.ToString());
    }

    protected override void PreStart()
    {
        using var activity =
            OpenTelemetryHelpers.ActivitySource.StartActivity(nameof(PreStart), ActivityKind.Server, _parentSpanId!);
        AddEvent(activity);
        base.PreStart();
    }

    public override void Start()
    {
        using var activity =
            OpenTelemetryHelpers.ActivitySource.StartActivity(nameof(Start), ActivityKind.Server, _parentSpanId!);
        AddEvent(activity);
        base.Start();
    }

    public override void SendMessage(Envelope message)
    {
        base.SendMessage(message);
    }

    // protected override ActorBase CreateNewActorInstance()
    // {
    //     using var activity = OpenTelemetryHelpers.ActivitySource.StartActivity(nameof(CreateNewActorInstance), ActivityKind.Server, _parentSpanId);
    //     AddEvent(activity);
    //     var res = base.CreateNewActorInstance();
    //     return res;
    // }

    private void AddEvent(Activity? activity, [CallerMemberName] string callerName = "")
    {
        activity?.AddEvent(new ActivityEvent(callerName));
        activity?.AddTag(OtelTags.ActorType, Props.Type.Name);
    }

    protected override void AutoReceiveMessage(Envelope envelope)
    {
        using var activity =
            OpenTelemetryHelpers.ActivitySource.StartActivity(nameof(AutoReceiveMessage), ActivityKind.Server,
                _parentSpanId!);
        AddEvent(activity);
        System.Hooks().ActorAutoReceiveMessage(envelope.Message, Self, envelope.Sender);
        base.AutoReceiveMessage(envelope);
    }

    public override void SendSystemMessage(ISystemMessage systemMessage)
    {
        using var activity =
            OpenTelemetryHelpers.ActivitySource.StartActivity(nameof(SendSystemMessage), ActivityKind.Server,
                _parentSpanId!);
        AddEvent(activity);
        activity?.AddEvent(new ActivityEvent("SystemMessage: " + systemMessage));
        System.Hooks().ActorSendSystemMessage(systemMessage, Self);
        base.SendSystemMessage(systemMessage);
    }
}