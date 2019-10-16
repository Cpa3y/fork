using Fork.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Sources
{
    public class TcpListenerSource : SourceBase
    {
        private readonly string connection;

        private Func<Task<IConnection>> factory;

        public TcpListenerSource(string connection)
        {
            this.connection = connection;
            factory = TcpFactory.Listen(connection);
        }

        protected override Task<IConnection> Connect()
        {
            // TODO: Need validation
            return factory();
        }
    }
}
