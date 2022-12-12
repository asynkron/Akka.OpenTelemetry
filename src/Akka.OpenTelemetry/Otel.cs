using Akka.Actor;

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
        //subscribe to IActorTelemetryEvent. new event type in Akka.NET
        //system.EventStream.Subscribe<Akka.Actor.IActorTelemetryEvent>()

        return new Otel();
    }
}