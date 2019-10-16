using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Connections
{
    public class TcpClientConnection : IConnection
    {

        private readonly TcpClient client;
        private readonly Stream stream;
        private readonly StreamReader reader;
        private readonly StreamWriter writer;

        public TcpClientConnection(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();
            this.reader = new StreamReader(stream);
            this.writer = new StreamWriter(stream);
        }


        public bool IsAlive => client.Connected;


        public Task<string> Read() => reader.ReadLineAsync();

        public async Task Write(string line)
        {
            await writer.WriteLineAsync(line);
            await writer.FlushAsync();
        }

    }
}
