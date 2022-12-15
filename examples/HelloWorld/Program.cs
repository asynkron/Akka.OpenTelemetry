using System.Diagnostics;
using Akka;
using Akka.Actor;
using Akka.OpenTelemetry.Telemetry;
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
    var props = Props.Create<MyActor>();
    var reff = system.ActorOf(props);

    reff.Tell(new SpawnChild());

    reff.Tell(new DoTell("Testing"));
    reff.Tell(new DoTell("Testing2"));
    reff.Tell(new DoTell("Testing3"));
    var x = await reff.Ask<AskResponse>(new AskRequest());
    reff.Tell(new DoTell("Testing4"));
    reff.Tell(new DoTell("Testing5"));


    reff.Tell(PoisonPill.Instance);
}

await Task.Delay(100);
tracerProvider!.ForceFlush();
Console.ReadLine();

public record DoTell(string Message);
public record AskResponse();

public record AskRequest();

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
                    var childProps = Props.Create<MyChildActor>();
                    var reff = Context.ActorOf(childProps);
                    reff.Tell("hello");
                    break;
                }
                case DoTell s:
                    Console.WriteLine("Got tell string: " + s.Message);
                    break;
                case AskRequest _:
                    Console.WriteLine("Got ask request");
                    Sender.Tell(new AskResponse());
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