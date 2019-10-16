using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fork.Core.Connections
{
    public class TcpClientConnection : IConnection
    {
        private readonly ILogger logger;

        private readonly TcpClient client;
        private readonly Lazy<Stream> stream;
        private readonly Lazy<StreamWriter> writer;
        private readonly Lazy<StreamReader> reader;


        public bool IsAlive => client.Connected;


        public TcpClientConnection(TcpClient client, ILogger logger)
        {
            this.client = client;
            this.stream = new Lazy<Stream>(() => client.GetStream(), LazyThreadSafetyMode.ExecutionAndPublication);
            this.writer = new Lazy<StreamWriter>(() => new StreamWriter(stream.Value), LazyThreadSafetyMode.ExecutionAndPublication);
            this.reader = new Lazy<StreamReader>(() => new StreamReader(stream.Value), LazyThreadSafetyMode.ExecutionAndPublication);
            this.logger = logger;
        }

        public async Task<ReadResult> Read(CancellationToken cancellationToken)
        {
            try
            {
                var line = await reader.Value.ReadLineAsync();
                return new ReadResult(line, line != null && client.Connected);
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "Exception while reading");
                return new ReadResult(null, false);
            }
        }

        public async Task<bool> Write(string line)
        {
            try
            {
                var w = writer.Value;
                await w.WriteLineAsync(line);
                w.Flush();
                return true;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception while transimmitting line");
                return false;
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
