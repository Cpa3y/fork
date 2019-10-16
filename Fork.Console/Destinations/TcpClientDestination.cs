using Fork.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Destinations
{
    public class TcpClientDestination : DestinationBase
    {
        private readonly string connection;

        public TcpClientDestination(string connection)
        {
            this.connection = connection;
        }


        protected override Task<IConnection> Connect()
        {
            return TcpFactory.Connect(connection);
        }
    }
}
