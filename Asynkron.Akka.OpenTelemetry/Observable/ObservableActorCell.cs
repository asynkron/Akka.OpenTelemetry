using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Dispatch;
using Asynkron.Akka.Decorators;

namespace Asynkron.Akka.Observable;

public class ObservableActorCell : DecoratorActorCell
{
    public ObservableActorCell(ActorSystemImpl system, IInternalActorRef self, Props props,
        MessageDispatcher dispatcher, IInternalActorRef parent) : base(system, self, props, dispatcher, parent)
    {
    }

    protected override void ReceiveMessage(object message)
    {
        //TODO: unwrap envelope message here
        //copy from Proto.Actor
        base.ReceiveMessage(message);
    }
}