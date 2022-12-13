using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Remote;
using Akka.Serialization;

namespace Akka.OpenTelemetry.Remote;

// public virtual Address DefaultAddress
// {
// get
// {
//     //HACK: race condition here. just spin until set
//     if (Inner.DefaultAddress is null) SpinWait.SpinUntil(() => Inner.DefaultAddress != null);
//     }


[ProxyInterfaceGenerator.Proxy(typeof(RemoteActorRefProvider2), false)]
public partial interface IRemoteActorRefProviderDecorator
{

}