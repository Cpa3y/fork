using Fork.Core.Connections;
using Fork.Core.Destinations;
using Fork.Core.Sources;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Core
{
    public class Fork
    {
        private readonly Source source;
        private readonly ILogger logger;
        private readonly List<Destination> destinations;

        public Fork(ILogger logger, Func<Task<IConnection>> sourceFactory, IEnumerable<(string id, Func<Task<IConnection>> factory)> destinationFactories, int bufferSize = -1)
        {
            this.logger = logger;
            this.source = new Source(logger.ForContext<Source>(), sourceFactory, x => OnReceivedFromSource(x));
            this.destinations = destinationFactories.Select(x => new Destination(x.id, x.factory, logger.ForContext<Destination>(), bufferSize)).ToList();

        }

        public DestinationState[] GetDestinationsState()
        {
            return destinations.Select(x => new DestinationState
            {
                CurrentBufferLength = x.BufferSize,
                WasBufferDrops = x.WasBufferDrops(),
                IsActive = x.IsActive,
                TransmittedCount = x.TransmittedCount
            }).ToArray();
        }

        public SourceState GetSourceState()
        {
            return new SourceState
            {
                IsActive = source.IsActive,
                ReceivedCount = source.ReceivedCount
            };

        }

        public void Run()
        {
            source.Listen();
        }

        private void OnReceivedFromSource(string line)
        {
            foreach (var d in destinations)
                d.Send(line);
        }
    }
}
