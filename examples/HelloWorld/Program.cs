using System.Diagnostics;
using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Logger.Extensions.Logging;
using Akka.OpenTelemetry.Telemetry;
using Demo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var rb = ResourceBuilder.CreateDefault().AddService("Akka.OpenTelemetry.HelloWorld");
var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(rb)
    .AddAkkaInstrumentation()
    .AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:4317");
        options.ExportProcessorType = ExportProcessorType.Batch;
    })
    .Build();


var services = new ServiceCollection();
services.AddLogging(l =>
{
    l.SetMinimumLevel(LogLevel.Debug);
    l.AddOpenTelemetry(
        options =>
        {
            options
                .SetResourceBuilder(rb)
                .AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri("http://localhost:4317");
                    o.ExportProcessorType = ExportProcessorType.Batch;
                });
        });
});

var serviceProvider = services.BuildServiceProvider();
var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
LoggingLogger.LoggerFactory = loggerFactory;



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
        private ILoggingAdapter _logger = Context.GetLogger();
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case SpawnChild:
                {
                    _logger.Info("Hello From OpenTelemetry");
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