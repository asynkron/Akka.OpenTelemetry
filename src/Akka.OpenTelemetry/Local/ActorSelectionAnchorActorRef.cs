using Akka.Actor;
using Akka.Dispatch.SysMsg;
using Akka.OpenTelemetry.Telemetry;
using Akka.Util;

namespace Akka.OpenTelemetry.Local;

public class ActorSelectionAnchorActorRef : IInternalActorRef
{
    private readonly IInternalActorRef _inner;

    public ActorSelectionAnchorActorRef(IInternalActorRef inner)
    {
        _inner = inner;
    }

    public void Tell(object message, IActorRef sender)
    {
        //if message is an ActorSelection message, unwrap it, extract current headers, and rebake a new one
        if (message is ActorSelectionMessage asl)
        {
            var m = asl.Message;
            var envelope = OpenTelemetryHelpers.ExtractHeaders(m, sender);
            var a = new ActorSelectionMessage(envelope, asl.Elements, asl.WildCardFanOut);
            _inner.Tell(a, sender);
            return;
        }

        _inner.Tell(message, sender);
    }

    public bool Equals(IActorRef? other)
    {
        return _inner.Equals(other);
    }

    public int CompareTo(IActorRef? other)
    {
        return _inner.CompareTo(other);
    }

    public ISurrogate ToSurrogate(ActorSystem system)
    {
        return _inner.ToSurrogate(system);
    }

    public int CompareTo(object? obj)
    {
        return _inner.CompareTo(obj);
    }

    public ActorPath Path => _inner.Path;
    public bool IsLocal => _inner.IsLocal;

    public IActorRef GetChild(IReadOnlyList<string> name)
    {
        return _inner.GetChild(name);
    }

    public void Resume(Exception? causedByFailure = null)
    {
        _inner.Resume(causedByFailure);
    }

    public void Start()
    {
        _inner.Start();
    }

    public void Stop()
    {
        _inner.Stop();
    }

    public void Restart(Exception cause)
    {
        _inner.Restart(cause);
    }

    public void Suspend()
    {
        _inner.Suspend();
    }

    public void SendSystemMessage(ISystemMessage message)
    {
        _inner.SendSystemMessage(message);
    }

    public IInternalActorRef Parent => _inner.Parent;
    public IActorRefProvider Provider => _inner.Provider;
#pragma warning disable CS0618
    public void SendSystemMessage(ISystemMessage message, IActorRef sender)
    {
        _inner.SendSystemMessage(message, sender);
    }

    public bool IsTerminated => _inner.IsTerminated;
#pragma warning restore CS0618
}