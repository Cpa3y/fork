using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fork.Sources
{
    public abstract class SourceBase : ISource
    {
        private IConnection connection;

        public async Task<string> Read(CancellationToken cancellation)
        {
            while (!(connection?.IsAlive ?? false))
                connection = await Connect();

            return await connection.Read();
        }

        protected abstract Task<IConnection> Connect();
    }
}
