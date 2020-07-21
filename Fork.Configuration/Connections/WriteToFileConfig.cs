using Fork.Core.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Configuration.Connections
{
    public class WriteToFileConfig : ConnectionConfig
    {
        public string Filename { set; get; }

        public override string Id => $"file='{Filename}'";

        public override Func<Task<IConnection>> GetFactory()
        {
            return () => Task.FromResult((IConnection)ConnectionFactory.CreateFile(Filename));
        }
    }
}
