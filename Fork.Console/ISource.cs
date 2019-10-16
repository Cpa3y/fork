using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fork
{
    public interface ISource
    {
        Task<string> Read(CancellationToken cancellation);
    }


    public interface IDestination
    {
        void Write(string line);
    }

}
