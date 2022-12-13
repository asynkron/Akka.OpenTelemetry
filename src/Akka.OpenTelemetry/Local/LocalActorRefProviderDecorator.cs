using Akka.Actor;
using Akka.Actor.Internal;
using Akka.Serialization;

namespace Akka.OpenTelemetry.Local;

[ProxyInterfaceGenerator.Proxy(typeof(LocalActorRefProvider))]
public partial interface ILocalActorRefProviderDecorator
{

}