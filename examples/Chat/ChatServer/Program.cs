//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2022 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2022 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka;
using Akka.Actor;
using Akka.Configuration;
using ChatMessages;
using ChatServer;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var config = ConfigurationFactory.ParseString("""
akka {
    actor {
        provider = "Akka.OpenTelemetry.Remote.OpenTelemetryRemoteActorRefProvider, Akka.OpenTelemetry"
    }
    remote {
        dot-netty.tcp {
            port = 8081
            hostname = 0.0.0.0
            public-hostname = localhost
        }
    }
}
""");

var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService("ChatServerApp")
    )
    .AddAkkaInstrumentation()
    .AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:4317");
        options.ExportProcessorType = ExportProcessorType.Batch;
    })
    .Build();

using var system = ActorSystem.Create("MyServer", config);
var props = Props.Create(() => new ChatServerActor());
system.ActorOf(props, "ChatServer");

tracerProvider?.ForceFlush();
Console.ReadLine();


namespace ChatServer
{
    internal class ChatServerActor : ReceiveActor, ILogReceive
    {
        private readonly HashSet<IActorRef> _clients = new();

        public ChatServerActor()
        {
            Receive<SayRequest>(message =>
            {
                var response = new SayResponse
                {
                    Username = message.Username,
                    Text = message.Text
                };
                foreach (var client in _clients) client.Tell(response, Self);
            });

            Receive<ConnectRequest>(message =>
            {
                _clients.Add(Sender);
                Console.WriteLine("Client connected " + Sender);
                Sender.Tell(new ConnectResponse
                {
                    Message = "Hello and welcome to Akka.NET chat example"
                }, Self);
            });

            Receive<NickRequest>(message =>
            {
                var response = new NickResponse
                {
                    OldUsername = message.OldUsername,
                    NewUsername = message.NewUsername
                };

                foreach (var client in _clients) client.Tell(response, Self);
            });
        }
    }
}