using Akka.Actor;
using Asynkron.Akka.Decorators;

namespace Asynkron.Akka.Observable;

public class ObservableActorRef : DecoratorActorRef
{
    public ObservableActorRef(IInternalActorRef inner) : base(inner)
    {
    }

    public override void Tell(object message, IActorRef sender)
    {
        //TODO: capture activity context
        //bake into envelope message
        //copy from Proto.Actor
        base.Tell(message, sender);
    }
}