using Akka.Actor;
using Akka.Configuration;
using Akka.OpenTelemetry;
using OpenTelemetry.Trace;

namespace Akka;

public static class TypeExtensions
{
    public static TracerProviderBuilder AddAkkaInstrumentation(this TracerProviderBuilder builder) =>
        builder.AddSource(OtelTags.ActivitySourceName);

    public static string GetTypeName(this object? message)
    {
        if (message is OpenTelemetryEnvelope envelope)
        {
            return envelope.Message.GetTypeName();
        }

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
}