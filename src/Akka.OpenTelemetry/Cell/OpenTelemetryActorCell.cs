using System.Diagnostics;
using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Dispatch;
using Akka.Dispatch.SysMsg;
using Akka.OpenTelemetry.Local;
using Akka.OpenTelemetry.Telemetry;

namespace Akka.OpenTelemetry.Cell;

public class OpenTelemetryActorCell : ActorCell, IActorRefFactory
{
    private readonly OpenTelemetrySettings _settings;

    private OpenTelemetryEnvelope? _currentEnvelope;
    public OpenTelemetrySettings Settings => _settings;

    public string ActorType => base.Actor?.ToString() ?? "<null>";

    public OpenTelemetryActorCell(ActorSystemImpl system, IInternalActorRef self, Props props,
        MessageDispatcher dispatcher, IInternalActorRef parent) : base(system, self, props, dispatcher, parent)
    {
        //TODO: where should this be injected?
        _settings = new OpenTelemetrySettings(true)
        {
            ParentSpanId = null,
            Context = this
        };
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

        var res = base.ActorOf(props, name);


        System.Hooks().ActorChildSpawned(_settings,props, res, Self);

        return res;
    }

    public new ActorSelection ActorSelection(string actorPath)
    {
        var selection = base.ActorSelection(actorPath);
        var anchor = new ActorSelectionAnchorActorRef((selection.Anchor as IInternalActorRef)!);
        var actorSelection = new ActorSelection(anchor, selection.Path);
        System.Hooks().ActorSelectionCreated(_settings,actorSelection, Self);
        return actorSelection;
    }

    public new ActorSelection ActorSelection(ActorPath actorPath)
    {
        var selection = base.ActorSelection(actorPath);
        var anchor = new ActorSelectionAnchorActorRef((selection.Anchor as IInternalActorRef)!);
        var actorSelection = new ActorSelection(anchor, selection.Path);
        System.Hooks().ActorSelectionCreated(_settings,actorSelection, Self);
        return actorSelection;
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



            //shady, yes, but we need to trigger receive timeout etc.
            ReceiveInner(envelope);

    }

    private void ReceiveInner(OpenTelemetryEnvelope envelope)
    {
        System.Hooks().ActorAroundReceiveMessage(_settings,envelope, Self, () => Invoke(new Envelope(envelope.Message, Sender, System)));
    }

    protected override void PreStart()
    {
        System.Hooks().ActorAroundPreStart(_settings,Self , () => base.PreStart());
    }

    public override void Start()
    {
        System.Hooks().ActorAroundStart(_settings,Self, () => base.Start());
    }

    protected override ActorBase CreateNewActorInstance()
    {
        var res = base.CreateNewActorInstance();
        System.Hooks().ActorCreateNewActorInstance(_settings,res, Self);
        return res;
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


    protected override void AutoReceiveMessage(Envelope envelope)
    {
        System.Hooks().ActorAroundAutoReceiveMessage(_settings, envelope.Message, Self, envelope.Sender,
            () => base.AutoReceiveMessage(envelope));
    }

    public override void SendSystemMessage(ISystemMessage systemMessage)
    {
        System.Hooks().ActorSendSystemMessage(_settings,systemMessage, Self);
        base.SendSystemMessage(systemMessage);
    }
}