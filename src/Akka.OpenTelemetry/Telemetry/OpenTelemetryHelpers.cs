using System.Diagnostics;
using System.Runtime.CompilerServices;
using Akka.Actor;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Akka.OpenTelemetry.Telemetry;


public static class OpenTelemetryHelpers
{
    public static readonly ActivitySource ActivitySource = new(OtelTags.ActivitySourceName);

    public static void DefaultSetupActivity(Activity _, object __)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Activity? BuildStartedActivity(
        ActivityContext parent,
        string source,
        string verb,
        object message,
        ActivitySetup activitySetup,
        ActivityKind activityKind = ActivityKind.Internal
    )
    {
        var messageType = message.GetTypeName();

        var name = $"Akka {source}.{verb} {messageType}";
        var tags = new[] { new KeyValuePair<string, object?>(OtelTags.MessageType, messageType) };
        var activity = ActivitySource.StartActivity(name, activityKind, parent, tags);

        if (activity is not null)
        {
            activitySetup(activity, message!);
        }

        return activity;
    }

    public static Dictionary<string, string> GetPropagationHeaders(this ActivityContext activityContext)
    {
        var context = new List<KeyValuePair<string, string>>();

        Propagators.DefaultTextMapPropagator.Inject(new PropagationContext(activityContext, Baggage.Current), context,
            AddHeader);

        return context.ToDictionary(x => x.Key, x => x.Value);
    }

    public static PropagationContext ExtractPropagationContext(this Dictionary<string, string> headers) =>
        Propagators.DefaultTextMapPropagator.Extract(default, headers,
            (dictionary, key) => dictionary.TryGetValue(key, out var value) ? new[] { value } : Array.Empty<string>()
        );

    private static void AddHeader(List<KeyValuePair<string, string>> list, string key, string value) =>
        list.Add(new KeyValuePair<string, string>(key, value));

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