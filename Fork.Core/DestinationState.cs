using System;
using System.Collections.Generic;
using System.Text;

namespace Fork.Core
{
    public class DestinationState
    {
        public int CurrentBufferLength { set; get; }
        public bool WasBufferDrops { set; get; }
        public long TransmittedCount { set; get; }
        public bool IsActive { set; get; }
             
    }
}
