using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fork
{
    public class TaskQueue<T>
    {
        private Task<T> prevTask = Task.FromResult(default(T));
        private object _lock = new object();


        public Task<T> Enqueue(Func<CancellationToken, T> func, CancellationToken ct)
        {
            lock (_lock)
            {
                return prevTask = prevTask.ContinueWith(x => func(ct), ct);
            }
        }

        public Task<T> Enqueue(Func<CancellationToken, Task<T>> func, CancellationToken ct)
        {
            lock (_lock)
            {
                return prevTask = prevTask.ContinueWith(x => func(ct), ct).Unwrap();
            }
        }

        private static async Task<T> Do(Task<T> prev, CancellationToken ct, Func<CancellationToken, Task<T>> func)
        {
            await prev.ContinueWith((x) => { }, ct);
            return await func(ct);
        }

    }

    public class TaskQueue
    {
        private Task prevTask = Task.CompletedTask;
        private object _lock = new object();


        public Task Enqueue(Action<CancellationToken> func, CancellationToken ct)
        {
            lock (_lock)
            {
                return prevTask = prevTask.ContinueWith(x => func(ct), ct);
            }
        }

        public Task Enqueue(Func<CancellationToken, Task> func, CancellationToken ct)
        {
            lock (_lock)
            {
                return prevTask = prevTask.ContinueWith(x => func(ct), ct).Unwrap();
            }
        }
    }
}
