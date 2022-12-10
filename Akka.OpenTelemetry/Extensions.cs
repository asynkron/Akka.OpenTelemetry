using Akka.Actor;
using Akka.OpenTelemetry;
using OpenTelemetry.Trace;

namespace Akka;

public static class TypeExtensions
{
    public static TracerProviderBuilder AddAkkaInstrumentation(this TracerProviderBuilder builder) =>
        builder.AddSource(OtelTags.ActivitySourceName);

    public static string GetMessageTypeName(this object? message)
    {
        return message?.GetType().Name ?? "null";
    }

    public static Props WithTracing(this Props self) => self.WithDeploy(new OpenTelemetryDeploy());
}