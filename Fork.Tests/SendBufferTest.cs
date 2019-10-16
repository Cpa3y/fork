using Fork.Core.Destinations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fork.Tests
{
    public class SendBufferTest
    {
        [Fact]
        public void Test1()
        {
            var l1 = new List<string>();
            var l2 = new List<int>();
            var buff = new SendBuffer<string>(async x => { l1.Add(x); l2.Add(Thread.CurrentThread.ManagedThreadId); await Task.Delay(100); }, x => { });

            buff.Enqueue("1");
            Thread.Sleep(10);

            buff.Enqueue("2");
            buff.Enqueue("3");
            buff.Enqueue("4");
            buff.Enqueue("5");
            buff.Enqueue("6");
            buff.Enqueue("7");
            buff.Enqueue("8");
            buff.Enqueue("9");
            buff.Enqueue("10");

            Thread.Sleep(5000);

            buff.Enqueue("11");
            buff.Enqueue("12");
            buff.Enqueue("13");
            Thread.Sleep(1000);

            Assert.Equal(new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" }, l1);
        }

    }
}
