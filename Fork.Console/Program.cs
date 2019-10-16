using Fork.Configuration.CommandLine;
using Fork.Destinations;
using Fork.Sources;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fork
{
    class Program
    {
        async static Task Main(string[] args)
        {
            var logger = new Serilog.LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Async(x => x.File("fork.serilog", fileSizeLimitBytes: 104857600))
                .CreateLogger();

            Log.Logger = logger;

            var fork = CommandLineParser.Parse(args).CreateFork(logger);

            fork.Run();

            while (true)
            {
                await Task.Delay(1000);
                foreach(var s in fork.GetDestinationsState())
                {
                    Console.Write(s);
                    Console.Write("\t");
                }

                Console.Write("\r");
            }
        }


        private static Func<Task<Fork.Core.Connections.IConnection>> ParseConnection(string arg)
        {
            var kvp = Parse(arg);
            switch (kvp.Key)
            {
                case "connect":
                        return Parser.ParseConnect(kvp.Value, out var ip, out var port) 
                        ? Core.Connections.TcpConnectionFactory.Connect(ip, port)
                        : throw new ApplicationException("Arg parsing failed"); 
                        
                case "listen":
                    return Parser.ParseListen(kvp.Value, out port)
                    ? Core.Connections.TcpConnectionFactory.Listen(port)
                    : throw new ApplicationException("Arg parsing failed");

                default:
                    throw new ApplicationException("Arg parsing failed"); 

            }

        }


        private static IDestination[] ParseDestinations(string[] args)
        {
            return args
                .Skip(1)
                .Select(x => ParseDestination(x))
                .Prepend(new ConsoleDestination())
                .ToArray();
        }

        private static IDestination ParseDestination(string v)
        {
            var kvp = Parse(v);

            switch (kvp.Key)
            {
                case "connect":
                    return new TcpClientDestination(kvp.Value);

                case "listen":
                    return new TcpListenerDestination(kvp.Value);

                case "port":
                    return new SerialPortDestination(kvp.Value);

                default:
                    throw new NotImplementedException();
            }
        }

        private static ISource ParseSource(string v)
        {
            var kvp = Parse(v);

            switch (kvp.Key)
            {
                case "connect":
                    return new TcpClientSource(kvp.Value);

                case "listen":
                    return new TcpListenerSource(kvp.Value);

                case "port":
                    return new SerialPortSource(kvp.Value);

                default:
                    throw new NotImplementedException();
            }
        }

        private static KeyValuePair<string, string> Parse(string s)
        {
            var kvp = s.Split('=');
            if (kvp.Length != 2)
                throw new ApplicationException();

            return new KeyValuePair<string, string>(kvp[0], kvp[1]);
        }
    }
}
