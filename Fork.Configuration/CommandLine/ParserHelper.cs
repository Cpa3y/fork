using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Configuration.CommandLine
{
    public static class ParserHelper
    {
        public static bool ParseConnect(string input, out IPAddress ip, out int port)
        {
            ip = null;
            port = 0;

            var ipPortPair = input.Split(':');

            return ipPortPair.Length == 2 && IPAddress.TryParse(ipPortPair[0], out ip) && int.TryParse(ipPortPair[1], out port);
        }

        public static bool ParseListen(string input, out int port)
        {
            port = 0;
            return int.TryParse(input, out port);
        }
    }
}
