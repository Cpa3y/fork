using Fork.Core.Connections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fork.Tests
{
    public class DispenserTest
    {
        private readonly Dispenser<string> dispenser = new Dispenser<string>();

        [Fact]
        public void Test1()
        {
            Exception excep = null;

            Task.Run(async () =>
            {
                try
                {
                    //Assert.True(false);

                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var val = await dispenser.Get();
                    Assert.True(stopwatch.ElapsedMilliseconds > 1000);
                    Assert.Equal("value1", val);
                    stopwatch.Restart();
                    val = await dispenser.Get();
                    Assert.True(stopwatch.ElapsedMilliseconds > 1000);
                    Assert.Equal("value2", val);
                    await Task.Delay(1000);
                    stopwatch.Restart();
                    val = await dispenser.Get();
                    Assert.True(stopwatch.ElapsedMilliseconds < 100);
                    Assert.Equal("value10", val);

                    stopwatch.Restart();
                    val = await dispenser.Get();
                    Assert.True(stopwatch.ElapsedMilliseconds > 1000);
                    Assert.Equal("value11", val);
                }
                catch (Exception ex)
                {
                    excep = ex;
                }

            });
            

            Thread.Sleep(2000);
            dispenser.Put("value1");
            Thread.Sleep(2000);
            dispenser.Put("value2");
            dispenser.Put("value3");
            dispenser.Put("value4");
            dispenser.Put("value5");
            dispenser.Put("value6");
            dispenser.Put("value7");
            dispenser.Put("value8");
            dispenser.Put("value9");
            dispenser.Put("value10");
            Thread.Sleep(3000);
            dispenser.Put("value11");


            Thread.Sleep(2000);
            if (excep != null)
                throw excep;
        }
    }
}
