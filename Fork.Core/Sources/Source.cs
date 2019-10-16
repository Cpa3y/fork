using Fork.Core.Connections;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fork.Core.Sources
{
    public class Source
    {
        private readonly ILogger logger;
        private readonly Func<Task<IConnection>> connectionFactory;
        private readonly Action<string> onReceived;
        private Task listenerLoop;

        private long receivedCount = 0;
        private bool isActive;


        public long ReceivedCount => receivedCount;
        public bool IsActive => isActive;


        public Source(ILogger logger, Func<Task<IConnection>> connectionFactory, Action<string> onReceived)
        {
            this.logger = logger;
            this.connectionFactory = connectionFactory;
            this.onReceived = onReceived;
        }

        public void Listen()
        {
            listenerLoop = Task.Run(() => RunLoop());
        }


        private async Task RunLoop()
        {
            try
            {
                while (true)
                {
                    logger.Information("Acquiring connection");
                    var connection = await connectionFactory();
                    logger.Information("Connection is acquired");
                    ReadResult result;
                    isActive = true;
                    while (connection.IsAlive && (result = await connection.Read(CancellationToken.None)).IsOk)
                    {
                        onReceived(result.Line);
                        receivedCount++;
                    }

                    isActive = false;
                    connection.Dispose();

                    logger.Information("Connection is dropped");
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Receiving loop is terminated");
            }
        }
    }
}
