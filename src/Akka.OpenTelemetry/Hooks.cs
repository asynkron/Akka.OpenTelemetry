using Akka.Actor;
using Akka.Dispatch.SysMsg;

namespace Akka.OpenTelemetry;

public class Hooks
{
    public void ActorRestarted(IActorRef self)
    {

    }

    public void ActorStopped(IActorRef self)
    {

    }

    public void ActorReceivedMessage(object message, IActorRef self, Action callback)
    {
      callback();
    }

    public void ActorSendMessage(object message, IActorRef self, IActorRef target, IActorRef sender)
    {
      //  Console.WriteLine($"Sending message {message} from {actorRef} to {target} with sender {sender}");
    }

    public void ActorSendSystemMessage(ISystemMessage message, IActorRef self)
    {
      if (message is Terminate)
      {
        //HACK: actor isn't really stopped yet.
        //but good enough for triggering start / stop metrics
        //TODO: use IActorTelemetryEvent once released
        ActorStopped(self);
      }
      //  Console.WriteLine($"Sending system message {message} from {actorRef}");

    }

    public void ActorSpawned(Props props, IInternalActorRef self)
    {
      //  Console.WriteLine($"Spawned actor {actorRef} with props {props}");

    }

    public void ActorAutoReceiveMessage(object message, IActorRef self, IActorRef sender)
    {
      //  Console.WriteLine($"Auto received message {message} on {actorRef} from {sender}");

    }

    public void ActorChildSpawned(Props props, IActorRef child, IActorRef self)
    {
      //  Console.WriteLine($"Child {child} spawned with props {props} by parent {parent}");
    }

    public void ActorSelectionCreated(ActorSelection actorSelection, IActorRef self)
    {

    }

    public void ActorPreStart(IActorRef actorRef, Action callback)
    {
      callback();
    }

    public void ActorStart(IActorRef actorRef, Action callback)
    {
      callback();
    }

    public void ActorCreateNewActorInstance(ActorBase res, IActorRef actorRef)
    {

    }
}