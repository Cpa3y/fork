using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fork.Core.Connections
{
    public interface IConnection : IDisposable
    {
        bool IsAlive { get; }
        Task<ReadResult> Read(CancellationToken cancellationToken);
        Task<bool> Write(string line);
    }
}
