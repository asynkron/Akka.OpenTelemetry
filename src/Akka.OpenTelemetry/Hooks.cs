using System.Diagnostics;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.OpenTelemetry.Telemetry;

namespace Akka.OpenTelemetry;

public class Hooks
{
    public void ActorRestarted(OpenTelemetrySettings settings, IActorRef self)
    {
    }

    public void ActorStopped(OpenTelemetrySettings settings, IActorRef self)
    {
    }

    public void ActorAroundReceiveMessage(OpenTelemetrySettings settings, OpenTelemetryEnvelope envelope,
        IActorRef self, Action callback)
    {
        if (Activity.Current != null) settings.ParentSpanId = Activity.Current.Id!;

        var propagationContext = envelope.Headers.ExtractPropagationContext();
        var message = envelope.Message;
        var actorType = settings.Context.ActorType;
        using var activity =
            OpenTelemetryHelpers
                .BuildStartedActivity(
                    propagationContext.ActivityContext,
                    actorType,
                    "ReceiveMessage",
                    envelope.Message, OpenTelemetryHelpers.DefaultSetupActivity
                );

        settings.ParentSpanId = activity?.Id ?? "";

        activity?.SetTag(OtelTags.ActorType, settings.Context.Props.Type.Name);
        activity?.SetTag(OtelTags.MessageType, message.GetTypeName());
        activity?.SetTag(OtelTags.ActorRef, settings.Context.Self.ToString());
        activity?.SetTag(OtelTags.SenderActorRef, settings.Context.Sender.ToString());

        callback();
    }

    public void ActorSendMessage(OpenTelemetrySettings settings, object message, IActorRef self, IActorRef target,
        IActorRef sender)
    {
        //  Console.WriteLine($"Sending message {message} from {actorRef} to {target} with sender {sender}");
    }

    public void ActorSendSystemMessage(OpenTelemetrySettings settings, ISystemMessage message, IActorRef self)
    {
        using var activity =
            OpenTelemetryHelpers.ActivitySource.StartActivity("SendSystemMessage", ActivityKind.Server,
                settings.ParentSpanId!);
        activity?.AddTag(OtelTags.ActorType, settings.Context.Props.Type.Name);
        activity?.AddEvent(new ActivityEvent("SystemMessage: " + message.GetTypeName()));

        if (message is Terminate)
            //HACK: actor isn't really stopped yet.
            //but good enough for triggering start / stop metrics
            //TODO: use IActorTelemetryEvent once released
            ActorStopped(settings, self);
        //  Console.WriteLine($"Sending system message {message} from {actorRef}");
    }

    public void ActorSpawned(OpenTelemetrySettings settings, Props props, IInternalActorRef self)
    {
        //  Console.WriteLine($"Spawned actor {actorRef} with props {props}");
    }

    public void ActorAroundAutoReceiveMessage(OpenTelemetrySettings settings, object message, IActorRef self,
        IActorRef sender, Action callback)
    {
        using var activity =
            OpenTelemetryHelpers.ActivitySource.StartActivity("AutoReceiveMessage", ActivityKind.Server,
                settings.ParentSpanId!);
        activity?.AddTag(OtelTags.ActorType, settings.Context.Props.Type.Name);
        //  Console.WriteLine($"Auto received message {message} on {actorRef} from {sender}");
        callback();
    }

    public void ActorChildSpawned(OpenTelemetrySettings settings, Props props, IActorRef child, IActorRef self)
    {
        using var activity =
            OpenTelemetryHelpers.ActivitySource.StartActivity("ActorOf", ActivityKind.Server, settings.ParentSpanId!);
        activity?.AddTag(OtelTags.ActorType, settings.Context.Props.Type.Name);
        activity?.AddEvent(new ActivityEvent("Spawned Child: " + child));
        //  Console.WriteLine($"Child {child} spawned with props {props} by parent {parent}");
    }

    public void ActorSelectionCreated(OpenTelemetrySettings settings, ActorSelection actorSelection, IActorRef self)
    {
    }

    public void ActorAroundPreStart(OpenTelemetrySettings settings, IActorRef actorRef, Action callback)
    {
        using var activity =
            OpenTelemetryHelpers.ActivitySource.StartActivity("PreStart", ActivityKind.Server, settings.ParentSpanId!);
        activity?.AddTag(OtelTags.ActorType, settings.Context.Props.Type.Name);
        callback();
    }

    public void ActorAroundStart(OpenTelemetrySettings settings, IActorRef actorRef, Action callback)
    {
        using var activity =
            OpenTelemetryHelpers.ActivitySource.StartActivity("Start", ActivityKind.Server, settings.ParentSpanId!);
        activity?.AddEvent(new ActivityEvent("Start"));
        activity?.AddTag(OtelTags.ActorType, settings.Context.Props.Type.Name);
        callback();
    }

    public void ActorCreateNewActorInstance(OpenTelemetrySettings settings, ActorBase actorInstance, IActorRef actorRef)
    {
        Activity.Current?.AddEvent(new ActivityEvent("CreateNewActorInstance " + actorInstance.GetType().Name));
    }
}