using Fork.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Destinations
{
    public class SerialPortDestination : DestinationBase
    {
        private readonly string connection;

        public SerialPortDestination(string connection)
        {
            this.connection = connection;
        }

        protected override Task<IConnection> Connect()
        {
            return Task.FromResult(SerialPortFactory.Open(connection));
        }
    }
}
