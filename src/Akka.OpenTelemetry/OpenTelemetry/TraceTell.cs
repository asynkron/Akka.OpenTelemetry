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

        using var tellActivity = OpenTelemetryHelpers.BuildStartedActivity(activity.Context, actorRefTag, "Tell",
            message,
            OpenTelemetryHelpers.DefaultSetupActivity);
        tellActivity?.AddTag(OtelTags.ActorType, props.Type.Name);
        var headers = Activity.Current?.Context.GetPropagationHeaders();
        var envelope = new OpenTelemetryEnvelope(message, headers ?? Headers.Empty);
        return envelope;
    }

    public static object ExtractHeaders(object message)
    {
        if (message is OpenTelemetryEnvelope alreadyEnvelope)
        {
            return alreadyEnvelope;
        }

        //special case, apply envelope to inner message
        if (message is ActorSelectionMessage selectionMessage)
        {
            var innerMessage = selectionMessage.Message;
            var envelope = ExtractHeaders(innerMessage);
            var x = new ActorSelectionMessage(envelope, selectionMessage.Elements, selectionMessage.WildCardFanOut);
            return x;
        }
        else
        {
            var activity = Activity.Current;
            if (activity is null)
            {
                return new OpenTelemetryEnvelope(message, Headers.Empty);
            }

            var current = Activity.Current?.GetTagItem(OtelTags.ActorType)?.ToString() ?? "NoSender";
            using var tellActivity = OpenTelemetryHelpers.BuildStartedActivity(activity.Context, current , "Tell",
                message,
                OpenTelemetryHelpers.DefaultSetupActivity);
            tellActivity?.AddTag(OtelTags.ActorType, current);
            var headers = Activity.Current?.Context.GetPropagationHeaders();
            var envelope = new OpenTelemetryEnvelope(message, headers ?? Headers.Empty);
            return envelope;
        }
    }
}