using Akka.Actor;
using Akka.Util.Internal;

namespace Asynkron.Akka.Observable;

internal sealed class ObservableExtension : ExtensionIdProvider<Observable>
{
    public override Observable CreateExtension(ExtendedActorSystem system)
    {
        return new(system.Provider.AsInstanceOf<ObservableActorRefProvider>());
    }
}