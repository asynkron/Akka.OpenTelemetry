using System.Diagnostics;
using Akka.Actor;

namespace Akka.OpenTelemetry;

public static class TraceTell
{
    public static OpenTelemetryEnvelope ExtractHeaders(object message, Props props)
    {
        var activity = Activity.Current;
        if (activity is null)
        {
            return new OpenTelemetryEnvelope(message, Headers.Empty);
        }

        var actorRefTag = Activity.Current?.GetTagItem(OtelTags.ActorRef)?.ToString() ?? "NoSender";

        using var tellActivity = OpenTelemetryHelpers.BuildStartedActivity(activity.Context, actorRefTag, "Tell", message,
            OpenTelemetryHelpers.DefaultSetupActivity);
        tellActivity?.AddTag(OtelTags.ActorType, props.Type.Name);
        var headers = Activity.Current?.Context.GetPropagationHeaders();
        var envelope = new OpenTelemetryEnvelope(message, headers ?? Headers.Empty);
        return envelope;
    }
}