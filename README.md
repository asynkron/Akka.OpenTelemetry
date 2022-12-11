# Akka.OpenTelemetry

This is a library that provides OpenTelemetry instrumentation for Akka.NET.

Development status is currently alpha.
Do not try to use this in any form of production environment yet.


## Getting started

### Installation

Install the NuGet package `Akka.OpenTelemetry` into your Akka.NET application.

### Configuration

```csharp
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
    

//augment config with OpenTelemetry settings
var bootstrap = BootstrapSetup.Create().WithOpenTelemetry();
var system = ActorSystem.Create("my-system", bootstrap);
var props = Props.Create<MyActor>();
var reff = system.ActorOf(props);
```

Tracing is currently applied to all actors under `/user` path.