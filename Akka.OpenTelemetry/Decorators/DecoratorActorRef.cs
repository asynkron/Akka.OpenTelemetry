using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.Util;

namespace Asynkron.Akka.Decorators;

public abstract class DecoratorActorRef : IInternalActorRef
{
    private readonly IInternalActorRef _inner;
    private ICell? _cell;

    protected DecoratorActorRef(IInternalActorRef inner)
    {
        _inner = inner;
        _cell = _inner switch
        {
            ActorRefWithCell withCell => withCell.Underlying,
            _ => null
        };
    }

    public virtual void Tell(object message, IActorRef sender)
    {
        _inner.Tell(message, sender);
    }

    public virtual bool Equals(IActorRef? other)
    {
        return _inner.Equals(other);
    }

    public virtual int CompareTo(IActorRef? other)
    {
        return _inner.CompareTo(other);
    }

    public virtual ISurrogate ToSurrogate(ActorSystem system)
    {
        return _inner.ToSurrogate(system);
    }

    public virtual int CompareTo(object? obj)
    {
        return _inner.CompareTo(obj);
    }

    public virtual ActorPath Path => _inner.Path;
    public virtual bool IsLocal => _inner.IsLocal;

    public virtual IActorRef GetChild(IReadOnlyList<string> name)
    {
        return _inner.GetChild(name);
    }

    public virtual void Resume(Exception? causedByFailure = null)
    {
        _inner.Resume(causedByFailure);
    }

    public virtual void Start()
    {
        _inner.Start();
    }

    public virtual void Stop()
    {
        _inner.Stop();
    }

    public virtual void Restart(Exception cause)
    {
        _inner.Restart(cause);
    }

    public virtual void Suspend()
    {
        _inner.Suspend();
    }

    public virtual void SendSystemMessage(ISystemMessage message)
    {
        _inner.SendSystemMessage(message);
    }

    public virtual IInternalActorRef Parent => _inner.Parent;
    public virtual IActorRefProvider Provider => _inner.Provider;

#pragma warning disable CS0618
    public virtual void SendSystemMessage(ISystemMessage message, IActorRef sender)
    {
        _inner.SendSystemMessage(message, sender);
    }

    public virtual bool IsTerminated => _inner.IsTerminated;
#pragma warning restore CS0618
}