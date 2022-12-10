// See https://aka.ms/new-console-template for more information

// create a new actor system

using Akka;
using Akka.Actor;
using Akka.Configuration;

var bootstrap = BootstrapSetup.Create().WithConfig(
    ConfigurationFactory.ParseString("""
akka.actor.provider = "Akka.OpenTelemetry.OpenTelemetryActorRefProvider, Akka.OpenTelemetry"
"""));

var system = ActorSystem.Create("my-system", bootstrap);
var props = Props.Create<MyActor>().WithTracing();
var reff = system.ActorOf(props);

reff.Tell("Testing");

Console.ReadLine();





class MyActor : UntypedActor
{
    protected override void OnReceive(object message)
    {
        if (message is string s)
        {
            Console.WriteLine("Got message string: " + s);
        }
    }
}