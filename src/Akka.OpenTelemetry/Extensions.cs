using Akka.Actor;
using Akka.Configuration;
using Akka.OpenTelemetry;
using Akka.OpenTelemetry.Telemetry;
using OpenTelemetry.Trace;

namespace Akka;

public static class Extensions
{
    public static TracerProviderBuilder AddAkkaInstrumentation(this TracerProviderBuilder builder)
    {
        return builder.AddSource(OtelTags.ActivitySourceName);
    }

    public static string GetTypeName(this object? message)
    {
        if (message is OpenTelemetryEnvelope envelope) return envelope.Message.GetTypeName();

        return message?.GetType().Name ?? "null";
    }

    public static BootstrapSetup WithOpenTelemetry(this BootstrapSetup self)
    {
        var bootstrap = self.WithConfig(
            ConfigurationFactory.ParseString("""
akka.actor.provider = "Akka.OpenTelemetry.Local.OpenTelemetryLocalActorRefProvider, Akka.OpenTelemetry"
"""));

        return bootstrap;
    }

    public static Hooks Hooks(this ActorSystem system)
    {
        return Otel.For(system).Hooks;
    }
}