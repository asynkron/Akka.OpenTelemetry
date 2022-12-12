using Akka.Actor;

namespace Akka.OpenTelemetry;

public class Hooks
{
    public Task ActorSpawned(IActorContext context)
    {
        return Task.CompletedTask;
    }

    public Task ActorRestarted(IActorContext context)
    {
        return Task.CompletedTask;
    }

    public Task ActorStopped(IActorContext context)
    {
        return Task.CompletedTask;
    }

    public Task ActorReceivedMessage(IActorContext context, object message)
    {
        return Task.CompletedTask;
    }

    public Task ActorSentMessage(IActorContext context, object message, IActorRef target)
    {
        return Task.CompletedTask;
    }
}