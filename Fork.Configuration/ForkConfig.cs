using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Configuration
{
    public class ForkConfig
    {
        public ConnectionConfig Source { set; get; }
        public List<ConnectionConfig> Destinations {set;get;}
        public int DestinationsBufferSize { set; get; } = 1000;

        public Core.Fork CreateFork(ILogger logger)
        {
            return new Core.Fork(logger, Source.GetFactory(), Destinations.Select(x => (x.Id, x.GetFactory())), DestinationsBufferSize);
        }
    }
}
