using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fork.Core.Connections
{
    public class Dispenser<T> where T: class
    {
        private readonly object _lock = new object();

        private TaskCompletionSource<T> tcs;
        private T value;


        public void Put(T item)
        {
            TaskCompletionSource<T> temp = null;
            lock (_lock)
            {
                if (tcs != null)
                {
                    temp = tcs;
                    tcs = null;
                }
                else
                {
                    value = item;
                    return;
                }
            }

            if (temp != null)
                temp.SetResult(item);
        }

        public Task<T> Get()
        {
            lock (_lock)
            {
                if (value != null)
                {
                    var v = value;
                    value = null;
                    return Task.FromResult(v);
                }
                else
                {
                    tcs = new TaskCompletionSource<T>();
                    return tcs.Task;
                }
            }
        }
    } 
}
