using Fork.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Sources
{
    public class TcpClientSource : SourceBase
    {
        private readonly string connection;

        public TcpClientSource(string connection)
        {
            this.connection = connection;
        }


        protected override Task<IConnection> Connect()
        {
            // TODO: Need validation
            return TcpFactory.Connect(connection);
        }

    }
}
