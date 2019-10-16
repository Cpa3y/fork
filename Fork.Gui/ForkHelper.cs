using Fork.Configuration;
using Fork.Configuration.AppConfig;
using Fork.Gui.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Gui
{



    public static class ForkHelper
    {
        private static Random random = new Random();
        private static readonly string[] arr = new []{
            "..........",
            "|.........",
            "||........",
            "|||.......",
            "||||......",
            "|||||.....",
            "||||||....",
            "|||||||...",
            "||||||||..",
            "|||||||||.",
            "||||||||||",
            };


        private static Core.Fork fork;
        private static ObservableCollection<ViewEntry> instance;
        private static int bufferSize;


        public static ObservableCollection<ViewEntry> GetStatus()
        {
            return instance; 
            //new ObservableCollection<ViewEntry>
            //{
            //    new ViewEntry{
            //        Address = "Addr1",
            //        BufferState = new StringBuilder().Append('|', random.Next(10)).ToString().PadRight(10, '.'),
            //        Kind = "Source",
            //        State = random.Next(2) == 0
            //    },
            //    new ViewEntry{
            //        Address = "Addr2",
            //        BufferState = new StringBuilder().Append('|', random.Next(10)).ToString().PadRight(10, '.'),
            //        Kind = "Destination",
            //        State = random.Next(2) == 0
            //    },
            //    new ViewEntry{
            //        Address = "Addr3",
            //        BufferState = new StringBuilder().Append('|', random.Next(10)).ToString().PadRight(10, '.'),
            //        Kind = "Destination",
            //        State = random.Next(2) == 0
            //    },
            //    new ViewEntry{
            //        Address = "Addr4",
            //        BufferState = new StringBuilder().Append('|', random.Next(10)).ToString().PadRight(10, '.'),
            //        Kind = "Destination",
            //        State = random.Next(2) == 0
            //    },
            //    new ViewEntry{
            //        Address = "Addr5",
            //        BufferState = new StringBuilder().Append('|', random.Next(10)).ToString().PadRight(10, '.'),
            //        Kind = "Destination",
            //        State = random.Next(2) == 0
            //    },
            //};
        }


        public static void Init()
        {
            var logger = new Serilog.LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Async(x => x.File("fork.serilog", fileSizeLimitBytes: 104857600))
                .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                logger.Fatal(e.ExceptionObject as Exception, "Unhandled exception");
            };

            var forkConfig = AppConfigParser.Parse();
            bufferSize = forkConfig.DestinationsBufferSize;
            fork = forkConfig.CreateFork(logger);
            fork.Run();

            instance = new ObservableCollection<ViewEntry>(forkConfig.Destinations.Select(x =>
                new ViewEntry
                {
                    Address = x.Id,
                    BufferState = string.Empty,
                    Kind = "Destination",
                    State = true
                }).Prepend(new ViewEntry
                {
                    Address = forkConfig.Source.Id,
                    BufferState = string.Empty,
                    Kind = "Source",
                    State = true
                }));
        }


        public static void UpdateState()
        {
            var sourceState = fork.GetSourceState();
            var destinationsState = fork.GetDestinationsState();

            instance.First().Update(sourceState.ReceivedCount, sourceState.IsActive);

            for (var i = 0; i < destinationsState.Length; ++i)
                instance[i + 1].Update(
                    !destinationsState[i].WasBufferDrops, 
                    CalcBufferState(destinationsState[i].CurrentBufferLength),
                    destinationsState[i].TransmittedCount,
                    destinationsState[i].IsActive
                    );
            //foreach (var e in instance)
            //    e.Update(random.Next(2) == 0, arr[random.Next(10)]);
        }

        private static string CalcBufferState(int item1)
        {
            if (item1 == 0)
                return arr[0];
            var pos = item1 * 10 / bufferSize + 1;

            return arr[Math.Min(pos, arr.Length - 1)];
        }
    }
}
