using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Connections
{
    public class TcpFactory
    {
        public static async Task<IConnection> Connect(string connection)
        {
            var ipPortPair = connection.Split(':');

            if (ipPortPair.Length == 2 && IPAddress.TryParse(ipPortPair[0], out var adr) && int.TryParse(ipPortPair[1], out var port))
            {
                var client = new TcpClient();
                await client.ConnectAsync(adr, port);

                return new TcpClientConnection(client);
            }
            return null;
        }


        public static Func<Task<IConnection>> Listen(string connection)
        {
            if (int.TryParse(connection, out var port))
            {
                var listener = new TcpListener(IPAddress.Any, port);
                listener.Start(1);

                return async () =>
                {
                    var client = await listener.AcceptTcpClientAsync();
                    return new TcpClientConnection(client);

                };

            }

            return null;
        }
    }
}
