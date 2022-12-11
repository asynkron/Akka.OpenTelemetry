using System.Diagnostics;
using Akka;
using Akka.Actor;
using Akka.OpenTelemetry;
using Demo;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService("Akka.OpenTelemetry.HelloWorld")
    )
    .AddAkkaInstrumentation()
    .AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:4317");
        options.ExportProcessorType = ExportProcessorType.Batch;
    })
    .Build();

var source = OpenTelemetryHelpers.ActivitySource;
using (var activity = source.StartActivity("demo", ActivityKind.Client))
{
    activity?.SetTag("demo", "true");

    var bootstrap = BootstrapSetup.Create().WithOpenTelemetry();

    var system = ActorSystem.Create("my-system", bootstrap);
    var props = Props.Create<MyActor>().WithTracing();
    var reff = system.ActorOf(props);

    reff.Tell(new SpawnChild());
    reff.Tell("Testing");
    reff.Tell("Testing2");
    reff.Tell("Testing3");

    reff.Tell(PoisonPill.Instance);
    await Task.Delay(100);
}

tracerProvider!.ForceFlush();
Console.ReadLine();


namespace Demo
{
    internal record SpawnChild;

    internal class MyActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case SpawnChild:
                {
                    var childProps = Props.Create<MyChildActor>().WithTracing();
                    var reff = Context.ActorOf(childProps);
                    reff.Tell("hello");
                    break;
                }
                case string s:
                    Console.WriteLine("Got message string: " + s);
                    break;
            }
        }
    }

    internal class MyChildActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case string s:
                    Console.WriteLine("Got message string: " + s);
                    break;
            }
        }
    }
}