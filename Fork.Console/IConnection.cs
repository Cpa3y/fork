using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork
{
    public interface IConnection
    {
        bool IsAlive { get; }

        Task<string> Read();
        Task Write(string line);
    }
}
