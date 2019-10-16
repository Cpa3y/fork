using Fork.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fork.Tests
{
    public class ActionAggregatorTest
    {
        [Fact]
        public void Test1()
        {
            var list = new List<int>();
            var agg = new ActionAggregator<string>(async x => { list.Add(x.Count); await Task.Delay(1000); });

            agg.Queue("1");
            Thread.Sleep(100);
            agg.Queue("1");
            agg.Queue("1");
            agg.Queue("1");
            agg.Queue("1");
            Thread.Sleep(950);
            agg.Queue("1");

            Assert.Equal(2, list.Count);
            Thread.Sleep(1100);

            Assert.Equal(new []{ 1, 4, 1}, list);



        }
    }
}
