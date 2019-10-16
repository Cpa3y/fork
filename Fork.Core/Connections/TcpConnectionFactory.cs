using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Core.Connections
{
    public class TcpConnectionFactory
    {
        private static readonly ILogger logger = Log.ForContext<TcpConnectionFactory>();


        public static Func<Task<IConnection>> Connect(IPAddress address, int port)
        {

            return async () =>
            {
                IConnection connection;
                int failuresCount = 0;
                do
                {
                    logger.Information("Connecting... Attempt {0}", failuresCount);
                    connection = await EstablishConnection(address, port);
                    await Relax(failuresCount++);
                }
                while (connection == null);

                return connection;
            };
        }


        public static Func<Task<IConnection>> Listen(int port)
        {
            var tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start(1);
            logger.Information("Start listening incoming connections");

            var helper = new ListenerHelper<IConnection>(async () =>
            {
                while (true)
                {
                    try
                    {

                        logger.Information("Awaiting incoming connections");
                        var tcpClient = await tcpListener.AcceptTcpClientAsync();
                        logger.Information("Receiving incoming connection");

                        return new TcpClientConnection(tcpClient, logger.ForContext<TcpClientConnection>());
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Exception while listening connection");
                        // TODO: Log exception
                        await Task.Delay(5000);
                    }
                }
            });

            helper.Start();

            return () => helper.Request();

            //return async () =>
            //{
            //    while (true)
            //    {
            //        try
            //        {

            //            logger.Information("Awaiting incoming connections");
            //            var tcpClient = await tcpListener.AcceptTcpClientAsync();
            //            logger.Information("Receiving incoming connection");

            //            return new TcpClientConnection(tcpClient, logger.ForContext<TcpClientConnection>());
            //        }
            //        catch (Exception ex)
            //        {
            //            logger.Error(ex, "Exception while listening connection");
            //            // TODO: Log exception
            //            await Task.Delay(5000);
            //        }
            //    }
            //};
        }
             

        private static async Task<IConnection> EstablishConnection(IPAddress address, int port)
        {
            try
            {
                var tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(address, port);

                return new TcpClientConnection(tcpClient, logger.ForContext<TcpClientConnection>());

            }
            catch (Exception ex)
            {
                // TODO: Log exception
                logger.Error(ex, "Exception while establishing connection");

                return null;
            }
        }

        private static Task Relax(int attempt)
        {
            return Task.Delay(Math.Min(attempt, 60) * 1000);
        }
    }
}
