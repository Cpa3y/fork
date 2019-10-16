using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Fork.Core.Connections;

namespace Fork.Configuration.Connections
{
    public class ConnectConfig : ConnectionConfig
    {
        public IPAddress Ip { set; get; }
        public int Port { set; get; }

        public override string Id => $"connect={Ip}:{Port}";

        public override Func<Task<IConnection>> GetFactory()
        {
            return TcpConnectionFactory.Connect(Ip, Port);
        }
    }
}
