using Fork.Core.Connections;
using System;
using System.Threading.Tasks;

namespace Fork.Configuration
{
    public abstract class ConnectionConfig
    {
        public abstract string Id { get; }

        public abstract Func<Task<IConnection>> GetFactory();
    }
}