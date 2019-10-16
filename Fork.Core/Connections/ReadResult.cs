using System;
using System.Collections.Generic;
using System.Text;

namespace Fork.Core.Connections
{
    public class ReadResult
    {
        public ReadResult(string line, bool isOk)
        {
            this.Line = line;
            this.IsOk = isOk;
        }

        public string Line { get; }
        public bool IsOk { get; }
    }
}
