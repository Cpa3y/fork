using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fork.Core.Connections
{
    public class ConnectionFactory
    {
        public static FileConnection CreateFile(string filename)
        {
            return new FileConnection(filename, Log.ForContext<FileConnection>());
        }
    }
}
