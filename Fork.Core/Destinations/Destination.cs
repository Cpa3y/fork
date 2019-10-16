using Fork.Core.Connections;
using Fork.Core.Utility;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Core.Destinations
{
    public class Destination
    {
        private readonly ILogger logger;
             
        private readonly SendBuffer<string> buffer;
        private readonly ActionAggregator<string> logAggregator;

        private readonly Func<Task<IConnection>> connectionFactory;

        private IConnection connection;
        private long transmittedCount = 0;
        private bool isActive;

        public Destination(string id, Func<Task<IConnection>> connectionFactory, ILogger logger, int bufferSize = -1)
        {
            this.logger = logger;
            this.connectionFactory = connectionFactory;
            this.logAggregator = new ActionAggregator<string>(x => logger.Warning("Destination {0} has dropped {1} lines", id, x.Count));
            this.buffer = new SendBuffer<string>(x => SendToConnection(x), x => logAggregator.Queue(x),  bufferSize);
        }


        public int BufferSize => buffer.Size;
        public bool WasBufferDrops() => buffer.WasDroppedItems();
        public long TransmittedCount => transmittedCount;
        public bool IsActive => isActive;


        public void Send(string line)
        {
            buffer.Enqueue(line);
        }

        private async Task SendToConnection(string line)
        {
            while (true)
            {
                if (connection == null)
                {
                    logger.Information("Acquiring connection");
                    connection = await connectionFactory();
                    logger.Information("Connection is acquired");
                }
                isActive = true;
                if (connection.IsAlive && await connection.Write(line) && connection.IsAlive )
                {
                    transmittedCount++;
                    break;
                }
                else
                {
                    logger.Information("Failed to send line.");
                }

                isActive = false;
                connection.Dispose();
                connection = null;
            }
        }
    }
}
