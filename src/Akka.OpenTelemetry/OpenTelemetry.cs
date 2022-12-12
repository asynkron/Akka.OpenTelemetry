using Akka.Actor;
using Akka.OpenTelemetry.Local;
using Akka.Util.Internal;

namespace Akka.OpenTelemetry;

internal sealed class OpenTelemetry : IExtension
{
    public OpenTelemetry(OpenTelemetryLocalActorRefProvider provider)
    {
        Provider = provider;
    }

    public OpenTelemetryLocalActorRefProvider Provider { get; }

    public static OpenTelemetry For(ActorSystem system)
    {
        return system.WithExtension<OpenTelemetry, OpenTelemetryExtension>();
    }
}

internal sealed class OpenTelemetryExtension : ExtensionIdProvider<OpenTelemetry>
{
    public override OpenTelemetry CreateExtension(ExtendedActorSystem system)
    {
        return new OpenTelemetry(system.Provider.AsInstanceOf<OpenTelemetryLocalActorRefProvider>());
    }
}