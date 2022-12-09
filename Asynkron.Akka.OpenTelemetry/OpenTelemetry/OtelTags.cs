namespace Asynkron.Akka.OpenTelemetry;

public static class OtelTags
{
    public const string ActivitySourceName = "Akka.Actor";
    public const string MessageType = "akka.message-type";
    public const string ResponseMessageType = "akka.response-message-type";
    public const string TargetActorRef = "akka.target-actorref";
    public const string SenderActorRef = "akka.sender-actorref";
    public const string ActorRef = "akka.actorref";

    /// <summary>
    ///     Type of the current actor, when applicable
    /// </summary>
    public const string ActorType = "akka.actor-type";
}