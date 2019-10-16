using Fork.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Sources
{
    public class SerialPortSource : SourceBase
    {
        private readonly string connection;

        public SerialPortSource(string connection)
        {
            this.connection = connection;
        }

        protected override Task<IConnection> Connect()
        {
            return Task.FromResult(SerialPortFactory.Open(connection));
        }
    }
}
