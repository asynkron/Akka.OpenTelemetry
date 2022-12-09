using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using MessageHeader = System.Collections.Generic.Dictionary<string,string>;

namespace Asynkron.Akka.Observable;

internal static class OpenTelemetryHelpers
{
    private static readonly ActivitySource ActivitySource = new(OtelTags.ActivitySourceName);

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
        var messageType = message.GetMessageTypeName();

        var name = $"Akka {source}.{verb} {messageType}";
        var tags = new[] { new KeyValuePair<string, object?>(OtelTags.MessageType, messageType) };
        var activity = ActivitySource.StartActivity(name, activityKind, parent, tags);

        if (activity is not null)
        {
            activitySetup(activity, message!);
        }

        return activity;
    }

    public static IEnumerable<KeyValuePair<string, string>> GetPropagationHeaders(this ActivityContext activityContext)
    {
        var context = new List<KeyValuePair<string, string>>();

        Propagators.DefaultTextMapPropagator.Inject(new PropagationContext(activityContext, Baggage.Current), context,
            AddHeader);

        return context;
    }

    public static PropagationContext ExtractPropagationContext(this MessageHeader headers) =>
        Propagators.DefaultTextMapPropagator.Extract(default, headers,
            (dictionary, key) => dictionary.TryGetValue(key, out var value) ? new[] { value } : Array.Empty<string>()
        );

    private static void AddHeader(List<KeyValuePair<string, string>> list, string key, string value) =>
        list.Add(new KeyValuePair<string, string>(key, value));
}