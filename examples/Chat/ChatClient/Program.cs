﻿//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2022 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2022 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Actor;
using Akka.Configuration;
using ChatMessages;

var config = ConfigurationFactory.ParseString("""
akka {
    actor {
        provider = "Akka.OpenTelemetry.Remote.OpenTelemetryRemoteActorRefProvider, Akka.OpenTelemetry"
    }
    remote {
        dot-netty.tcp {
		    port = 0
		    hostname = localhost
        }
    }
}
""");

using var system = ActorSystem.Create("MyClient", config);
var chatClient = system.ActorOf(Props.Create<ChatClientActor>());
chatClient.Tell(new ConnectRequest
{
    Username = "Roggan"
});

while (true)
{
    var input = Console.ReadLine();
    if (input.StartsWith("/"))
    {
        var parts = input.Split(' ');
        var cmd = parts[0].ToLowerInvariant();
        var rest = string.Join(" ", parts.Skip(1));

        if (cmd == "/nick")
            chatClient.Tell(new NickRequest
            {
                NewUsername = rest
            });
        if (cmd == "/exit")
        {
            Console.WriteLine("exiting");
            break;
        }
    }
    else
    {
        chatClient.Tell(new SayRequest
        {
            Text = input
        });
    }
}

system.Terminate().Wait();


internal class ChatClientActor : ReceiveActor, ILogReceive
{
    private readonly ActorSelection _server =
        Context.ActorSelection("akka.tcp://MyServer@localhost:8081/user/ChatServer");

    private string _nick = "Roggan";

    public ChatClientActor()
    {
        Receive<ConnectRequest>(cr =>
        {
            Console.WriteLine("Connecting....");
            _server.Tell(cr);
        });

        Receive<ConnectResponse>(rsp =>
        {
            Console.WriteLine("Connected!");
            Console.WriteLine(rsp.Message);
        });

        Receive<NickRequest>(nr =>
        {
            nr.OldUsername = _nick;
            Console.WriteLine("Changing nick to {0}", nr.NewUsername);
            _nick = nr.NewUsername;
            _server.Tell(nr);
        });

        Receive<NickResponse>(nrsp =>
        {
            Console.WriteLine("{0} is now known as {1}", nrsp.OldUsername, nrsp.NewUsername);
        });

        Receive<SayRequest>(sr =>
        {
            sr.Username = _nick;
            _server.Tell(sr);
        });

        Receive<SayResponse>(srsp => { Console.WriteLine("{0}: {1}", srsp.Username, srsp.Text); });
    }
}