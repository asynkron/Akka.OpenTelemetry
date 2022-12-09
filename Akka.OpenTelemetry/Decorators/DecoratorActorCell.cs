using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Dispatch;

namespace Asynkron.Akka.Decorators;

public abstract class DecoratorActorCell : ActorCell
{
    protected DecoratorActorCell(ActorSystemImpl system, IInternalActorRef self, Props props,
        MessageDispatcher dispatcher, IInternalActorRef parent) : base(system, self, props, dispatcher, parent)
    {
    }

    public override IActorRef ActorOf(Props props, string? name = null)
    {
        var res = base.ActorOf(props, name);
        if (res is ActorRefWithCell cell)
        {
            //TODO: in a galaxy far far away, a long time ago, I knew this shit
        }

        return res;
    }
}