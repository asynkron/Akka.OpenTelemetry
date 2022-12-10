using Akka;
using Akka.Actor;
using Akka.Configuration;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


var builder = ResourceBuilder.CreateDefault();

var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(builder
        .AddService("Proto.Cluster.Tests")
    )
    .AddAkkaInstrumentation()
    .AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:4317");
        options.ExportProcessorType = ExportProcessorType.Batch;
    })
    .Build();


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