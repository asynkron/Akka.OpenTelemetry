using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Serialization;

namespace Asynkron.Akka.Decorators;

public abstract class DecoratorActorRefProvider : IActorRefProvider
{
    private readonly IActorRefProvider _inner;

    protected DecoratorActorRefProvider(IActorRefProvider inner)
    {
        _inner = inner;
    }

    public virtual IActorRef DeadLetters => _inner.DeadLetters;
    public virtual IActorRef IgnoreRef => _inner.IgnoreRef;
    public virtual ActorPath RootPath => _inner.RootPath;
    public virtual Settings Settings => _inner.Settings;
    public virtual Deployer Deployer => _inner.Deployer;
    public virtual IInternalActorRef TempContainer => _inner.TempContainer;
    public virtual Task TerminationTask => _inner.TerminationTask;
    public virtual Address DefaultAddress => _inner.DefaultAddress;
    public virtual Information SerializationInformation => _inner.SerializationInformation;
    public virtual IInternalActorRef RootGuardian => _inner.RootGuardian;
    public virtual LocalActorRef Guardian => _inner.Guardian;
    public virtual LocalActorRef SystemGuardian => _inner.SystemGuardian;

    public virtual IActorRef RootGuardianAt(Address address)
    {
        return _inner.RootGuardianAt(address);
    }

    public virtual void Init(ActorSystemImpl system)
    {
        _inner.Init(system);
    }

    public virtual ActorPath TempPath()
    {
        return _inner.TempPath();
    }

    public virtual void RegisterTempActor(IInternalActorRef actorRef, ActorPath path)
    {
        _inner.RegisterTempActor(actorRef, path);
    }

    public virtual void UnregisterTempActor(ActorPath path)
    {
        _inner.UnregisterTempActor(path);
    }

    public virtual FutureActorRef<T> CreateFutureRef<T>(TaskCompletionSource<T> tcs)
    {
        return _inner.CreateFutureRef(tcs);
    }

    public virtual IInternalActorRef ActorOf(ActorSystemImpl system, Props props, IInternalActorRef supervisor,
        ActorPath path, bool systemService, Deploy deploy, bool lookupDeploy, bool async)
    {
        return _inner.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);
    }

    public virtual IActorRef ResolveActorRef(string path)
    {
        return _inner.ResolveActorRef(path);
    }

    public virtual IActorRef ResolveActorRef(ActorPath actorPath)
    {
        return _inner.ResolveActorRef(actorPath);
    }

    public virtual Address GetExternalAddressFor(Address address)
    {
        return _inner.GetExternalAddressFor(address);
    }
}