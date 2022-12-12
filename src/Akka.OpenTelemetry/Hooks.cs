using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.OpenTelemetry.Telemetry;

namespace Akka.OpenTelemetry;

public class Hooks
{
    public void ActorRestarted(OpenTelemetrySettings settings, IActorRef self)
    {

    }

    public void ActorStopped(OpenTelemetrySettings settings,IActorRef self)
    {

    }

    public void ActorAroundReceiveMessage(OpenTelemetrySettings settings,object message, IActorRef self, Action callback)
    {
      callback();
    }

    public void ActorSendMessage(OpenTelemetrySettings settings,object message, IActorRef self, IActorRef target, IActorRef sender)
    {
      //  Console.WriteLine($"Sending message {message} from {actorRef} to {target} with sender {sender}");
    }

    public void ActorSendSystemMessage(OpenTelemetrySettings settings,ISystemMessage message, IActorRef self)
    {
      if (message is Terminate)
      {
        //HACK: actor isn't really stopped yet.
        //but good enough for triggering start / stop metrics
        //TODO: use IActorTelemetryEvent once released
        ActorStopped(settings, self);
      }
      //  Console.WriteLine($"Sending system message {message} from {actorRef}");

    }

    public void ActorSpawned(OpenTelemetrySettings settings,Props props, IInternalActorRef self)
    {
      //  Console.WriteLine($"Spawned actor {actorRef} with props {props}");

    }

    public void ActorAroundAutoReceiveMessage(OpenTelemetrySettings settings,object message, IActorRef self, IActorRef sender, Action callback)
    {
      //  Console.WriteLine($"Auto received message {message} on {actorRef} from {sender}");
      callback();
    }

    public void ActorChildSpawned(OpenTelemetrySettings settings,Props props, IActorRef child, IActorRef self)
    {
      //  Console.WriteLine($"Child {child} spawned with props {props} by parent {parent}");
    }

    public void ActorSelectionCreated(OpenTelemetrySettings settings,ActorSelection actorSelection, IActorRef self)
    {

    }

    public void ActorAroundPreStart(OpenTelemetrySettings settings,IActorRef actorRef, Action callback)
    {
      callback();
    }

    public void ActorAroundStart(OpenTelemetrySettings settings,IActorRef actorRef, Action callback)
    {
      callback();
    }

    public void ActorCreateNewActorInstance(OpenTelemetrySettings settings,ActorBase res, IActorRef actorRef)
    {

    }
}