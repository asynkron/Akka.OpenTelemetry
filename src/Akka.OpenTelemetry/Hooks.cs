using Akka.Actor;
using Akka.Dispatch.SysMsg;

namespace Akka.OpenTelemetry;

public class Hooks
{
    public void ActorRestarted(IActorRef actorRef)
    {

    }

    public void ActorStopped(IActorRef actorRef)
    {

    }

    public void ActorReceivedMessage(object message, IActorRef actorRef)
    {
      //  Console.WriteLine($"Received message {message} on {actorRef}");

    }

    public void ActorSendMessage(object message, IActorRef actorRef, IActorRef target, IActorRef sender)
    {
      //  Console.WriteLine($"Sending message {message} from {actorRef} to {target} with sender {sender}");
    }

    public void ActorSendSystemMessage(ISystemMessage message, IActorRef actorRef)
    {
      //  Console.WriteLine($"Sending system message {message} from {actorRef}");

    }

    public void ActorSpawned(Props props, IInternalActorRef actorRef)
    {
      //  Console.WriteLine($"Spawned actor {actorRef} with props {props}");

    }

    public void ActorAutoReceiveMessage(object message, IActorRef actorRef, IActorRef sender)
    {
      //  Console.WriteLine($"Auto received message {message} on {actorRef} from {sender}");

    }
}