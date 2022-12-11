using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Serialization;

namespace Akka.Decorators;

public abstract class ActorRefProviderDecorator : IActorRefProvider
{
    protected IActorRefProvider _localProvider = default!;


    protected IActorRefProvider LocalProvider => _localProvider;

    public virtual IActorRef DeadLetters => LocalProvider.DeadLetters;
    public virtual IActorRef IgnoreRef => LocalProvider.IgnoreRef;
    public virtual ActorPath RootPath => LocalProvider.RootPath;
    public virtual Settings Settings => LocalProvider.Settings;
    public virtual Deployer Deployer => LocalProvider.Deployer;
    public virtual IInternalActorRef TempContainer => LocalProvider.TempContainer;
    public virtual Task TerminationTask => LocalProvider.TerminationTask;
    public virtual Address DefaultAddress => LocalProvider.DefaultAddress;
    public virtual Information SerializationInformation => LocalProvider.SerializationInformation;
    public virtual IInternalActorRef RootGuardian => LocalProvider.RootGuardian;
    public virtual LocalActorRef Guardian => LocalProvider.Guardian;
    public virtual LocalActorRef SystemGuardian => LocalProvider.SystemGuardian;
    public virtual IActorRef RootGuardianAt(Address address) => LocalProvider.RootGuardianAt(address);
    public virtual void Init(ActorSystemImpl system) => LocalProvider.Init(system);
    public virtual ActorPath TempPath() => LocalProvider.TempPath();
    public virtual void RegisterTempActor(IInternalActorRef actorRef, ActorPath path) => LocalProvider.RegisterTempActor(actorRef, path);
    public virtual void UnregisterTempActor(ActorPath path) => LocalProvider.UnregisterTempActor(path);
    public virtual FutureActorRef<T> CreateFutureRef<T>(TaskCompletionSource<T> tcs) => LocalProvider.CreateFutureRef(tcs);
    public virtual IInternalActorRef ActorOf(ActorSystemImpl system, Props props, IInternalActorRef supervisor, ActorPath path, bool systemService, Deploy deploy, bool lookupDeploy, bool async) => LocalProvider.ActorOf(system, props, supervisor, path, systemService, deploy, lookupDeploy, async);
    public virtual IActorRef ResolveActorRef(string path) => LocalProvider.ResolveActorRef(path);
    public virtual IActorRef ResolveActorRef(ActorPath actorPath) => LocalProvider.ResolveActorRef(actorPath);
    public virtual Address GetExternalAddressFor(Address address) => LocalProvider.GetExternalAddressFor(address);
}