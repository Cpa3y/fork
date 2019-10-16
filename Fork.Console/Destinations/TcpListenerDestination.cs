using Fork.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Destinations
{
    public class TcpListenerDestination : DestinationBase
    {
        private readonly string connection;
        private readonly Func<Task<IConnection>> factory;

        public TcpListenerDestination(string connection)
        {
            this.connection = connection;
            factory = TcpFactory.Listen(connection); 
        }

        protected override Task<IConnection> Connect()
        {
            return factory();
        }
    }
}
