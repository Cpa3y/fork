using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fork.Core.Connections;

namespace Fork.Configuration.Connections
{
    public class ListenConfig : ConnectionConfig
    {
        public int Port { set; get; }
        public override string Id => $"listen={Port}";

        public override Func<Task<IConnection>> GetFactory()
        {
            return TcpConnectionFactory.Listen(Port);
        }
    }
}
