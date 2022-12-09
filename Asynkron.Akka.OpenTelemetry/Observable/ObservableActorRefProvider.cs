using Akka.Actor;
using Akka.Actor.Internal;
using Asynkron.Akka.Decorators;

namespace Asynkron.Akka.Observable;

public sealed class ObservableActorRefProvider : DecoratorActorRefProvider
{
    public ObservableActorRefProvider(IActorRefProvider inner) : base(inner)
    {
    }

    public override IInternalActorRef ActorOf(ActorSystemImpl system, Props props, IInternalActorRef supervisor,
        ActorPath path,
        bool systemService, Deploy deploy, bool lookupDeploy, bool async)
    {
        var reff = base.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);
        return new ObservableActorRef(reff);
    }
}