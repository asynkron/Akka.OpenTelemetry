using Akka.Actor;
using Akka.OpenTelemetry.Local;
using Akka.Util.Internal;

namespace Akka.OpenTelemetry;

internal sealed class OpenTelemetryExtension : ExtensionIdProvider<OpenTelemetry>
{
    public override OpenTelemetry CreateExtension(ExtendedActorSystem system)
    {
        return new(system.Provider.AsInstanceOf<OpenTelemetryLocalActorRefProvider>());
    }
}