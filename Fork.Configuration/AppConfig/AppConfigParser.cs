using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Configuration.AppConfig
{
    public static class AppConfigParser
    {
        public static ForkConfig Parse()
        {
            var config = CommandLine.CommandLineParser.Parse(ConfigurationManager.AppSettings["Fork:ConfigLine"].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            if (int.TryParse(ConfigurationManager.AppSettings["Fork:DestinationBuffersSize"], out var val))
                config.DestinationsBufferSize = val;

            return config;
        }
    }
}
