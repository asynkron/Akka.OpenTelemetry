using Akka.Actor;
using Akka.OpenTelemetry;

namespace Akka;

public static class TypeExtensions
{
    public static string GetMessageTypeName(this object? message)
    {
        return message?.GetType().Name ?? "null";
    }

    public static Props WithTracing(this Props self) => self.WithDeploy(new OpenTelemetryDeploy());
}