using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Destinations
{
    public class ConsoleDestination : IDestination
    {
        public void Write(string line)
        {
            Console.WriteLine(line);
        }
    }
}
