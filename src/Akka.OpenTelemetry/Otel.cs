using Akka.Actor;
using Akka.OpenTelemetry.Local;
using Akka.Util.Internal;

namespace Akka.OpenTelemetry;

internal sealed class Otel : IExtension
{
    public Otel()
    {
        Hooks = new Hooks();
    }

    public Hooks Hooks { get; }

    public static Otel For(ActorSystem system)
    {
        return system.WithExtension<Otel, OpenTelemetryExtension>();
    }
}

internal sealed class OpenTelemetryExtension : ExtensionIdProvider<Otel>
{
    public override Otel CreateExtension(ExtendedActorSystem system)
    {
        return new Otel();
    }
}