using Akka.Actor;
using Akka.OpenTelemetry.Local;

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