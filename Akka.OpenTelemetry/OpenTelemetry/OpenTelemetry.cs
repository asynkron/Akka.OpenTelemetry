using Akka.Actor;

namespace Akka.OpenTelemetry;

internal sealed class OpenTelemetry : IExtension
{
    public OpenTelemetry(OpenTelemetryActorRefProvider provider)
    {
        Provider = provider;
    }

    public OpenTelemetryActorRefProvider Provider { get; }

    public static OpenTelemetry For(ActorSystem system)
    {
        return system.WithExtension<OpenTelemetry, OpenTelemetryExtension>();
    }
}