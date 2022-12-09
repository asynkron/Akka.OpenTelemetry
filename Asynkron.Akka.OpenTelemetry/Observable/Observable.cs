using Akka.Actor;

namespace Asynkron.Akka.Observable;

internal sealed class Observable : IExtension
{
    public Observable(ObservableActorRefProvider provider)
    {
        Provider = provider;
    }

    public ObservableActorRefProvider Provider { get; }

    public static Observable For(ActorSystem system)
    {
        return system.WithExtension<Observable, ObservableExtension>();
    }
}