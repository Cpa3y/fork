using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fork.Destinations
{
    public abstract class DestinationBase : IDestination
    {
        private IConnection connection;
        private readonly TaskQueue queue = new TaskQueue();

        public void Write(string line)
        {
            queue.Enqueue(x => WriteAsync(line), CancellationToken.None);
        }


        private async Task WriteAsync(string line)
        {
            while (!(connection?.IsAlive ?? false))
                connection = await Connect();

            connection?.Write(line);
        }

        protected abstract Task<IConnection> Connect();
    }
}
