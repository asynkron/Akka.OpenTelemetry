using System.Diagnostics;
using Akka;
using Akka.Actor;
using Akka.Configuration;
using Akka.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService("Akka.OpenTelemetry.Demo")
    )
    .AddAkkaInstrumentation()
    .AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:4317");
        options.ExportProcessorType = ExportProcessorType.Batch;
    })
    .Build();

var source = OpenTelemetryHelpers.ActivitySource;
using (var activity =source.StartActivity("demo", ActivityKind.Client))
{
    activity?.SetTag("demo", "true");

    var bootstrap = BootstrapSetup.Create().WithConfig(
        ConfigurationFactory.ParseString("""
akka.actor.provider = "Akka.OpenTelemetry.OpenTelemetryActorRefProvider, Akka.OpenTelemetry"
"""));

    var system = ActorSystem.Create("my-system", bootstrap);
    var props = Props.Create<MyActor>().WithTracing();
    var reff = system.ActorOf(props);

    reff.Tell("Testing");
    reff.Tell("Testing2");
    reff.Tell("Testing3");

    Console.ReadLine();
}

tracerProvider.ForceFlush();
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