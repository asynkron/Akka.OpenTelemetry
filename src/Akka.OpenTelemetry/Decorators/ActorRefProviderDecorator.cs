using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Serialization;

namespace Akka.Decorators;

public abstract class ActorRefProviderDecorator : IActorRefProvider
{
    protected IActorRefProvider _inner = default!;


    protected IActorRefProvider Inner => _inner;

    public virtual IActorRef DeadLetters => Inner.DeadLetters;
    public virtual IActorRef IgnoreRef => Inner.IgnoreRef;
    public virtual ActorPath RootPath => Inner.RootPath;
    public virtual Settings Settings => Inner.Settings;
    public virtual Deployer Deployer => Inner.Deployer;
    public virtual IInternalActorRef TempContainer => Inner.TempContainer;
    public virtual Task TerminationTask => Inner.TerminationTask;
    public virtual Address DefaultAddress => Inner.DefaultAddress;
    public virtual Information SerializationInformation => Inner.SerializationInformation;
    public virtual IInternalActorRef RootGuardian => Inner.RootGuardian;
    public virtual LocalActorRef Guardian => Inner.Guardian;
    public virtual LocalActorRef SystemGuardian => Inner.SystemGuardian;
    public virtual IActorRef RootGuardianAt(Address address) => Inner.RootGuardianAt(address);
    public virtual void Init(ActorSystemImpl system) => Inner.Init(system);
    public virtual ActorPath TempPath() => Inner.TempPath();
    public virtual void RegisterTempActor(IInternalActorRef actorRef, ActorPath path) => Inner.RegisterTempActor(actorRef, path);
    public virtual void UnregisterTempActor(ActorPath path) => Inner.UnregisterTempActor(path);
    public virtual FutureActorRef<T> CreateFutureRef<T>(TaskCompletionSource<T> tcs) => Inner.CreateFutureRef(tcs);
    public virtual IInternalActorRef ActorOf(ActorSystemImpl system, Props props, IInternalActorRef supervisor, ActorPath path, bool systemService, Deploy deploy, bool lookupDeploy, bool async) => Inner.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);
    public virtual IActorRef ResolveActorRef(string path) => Inner.ResolveActorRef(path);
    public virtual IActorRef ResolveActorRef(ActorPath actorPath) => Inner.ResolveActorRef(actorPath);
    public virtual Address GetExternalAddressFor(Address address) => Inner.GetExternalAddressFor(address);
}