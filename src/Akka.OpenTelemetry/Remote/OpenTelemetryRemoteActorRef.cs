using Akka.Actor;
using Akka.Actor.Internal;
using Akka.OpenTelemetry.Telemetry;
using Akka.Remote;

namespace Akka.OpenTelemetry.Remote;

public class OpenTelemetryRemoteActorRef : RemoteActorRef
{
    public OpenTelemetryRemoteActorRef(RemoteTransport remote, Address localAddressToUse, ActorPath path,
        IInternalActorRef parent, Props props, Deploy deploy) : base(remote, localAddressToUse, path, parent, props,
        deploy)
    {
    }

    protected override void TellInternal(object message, IActorRef sender)
    {
        var envelope = OpenTelemetryHelpers.ExtractHeaders(message);
        if (InternalCurrentActorCellKeeper.Current != null)
        {
            var system = InternalCurrentActorCellKeeper.Current.System;
            var self = InternalCurrentActorCellKeeper.Current.Self;
            system.Hooks().ActorSendMessage(message, self, this, sender);
        }
        base.TellInternal(envelope, sender);
    }
}