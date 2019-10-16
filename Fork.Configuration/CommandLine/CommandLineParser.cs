using Fork.Configuration.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Configuration.CommandLine
{
    public static class CommandLineParser
    {
        public static ForkConfig Parse(string[] args)
        {
            return new ForkConfig
            {
                Source = ParseConnection(args.First()),
                Destinations = args.Skip(1).Select(x => ParseConnection(x)).ToList()
            };
        }



        private static ConnectionConfig ParseConnection(string arg)
        {
            var kvp = Parse(arg);
            switch (kvp.Key)
            {
                case "connect":
                    return ParserHelper.ParseConnect(kvp.Value, out var ip, out var port)
                    ? new ConnectConfig { Ip = ip, Port = port}
                    : throw new ApplicationException("Arg parsing failed");

                case "listen":
                    return ParserHelper.ParseListen(kvp.Value, out port)
                    ? new ListenConfig { Port = port}
                    : throw new ApplicationException("Arg parsing failed");

                default:
                    throw new ApplicationException("Arg parsing failed");

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
