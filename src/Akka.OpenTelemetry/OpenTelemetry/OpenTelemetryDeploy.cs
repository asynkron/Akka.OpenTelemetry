using Akka.Actor;

namespace Akka.OpenTelemetry;

public class OpenTelemetryDeploy : Deploy
{
    private readonly Deploy _fallback = null!;

    public OpenTelemetryDeploy()
    {

    }

    OpenTelemetryDeploy(Deploy fallback)
    {
        _fallback = fallback;
    }
    public override Deploy WithFallback(Deploy other)
    {
        return new OpenTelemetryDeploy(other);
    }
}