using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Core.Destinations
{
    public class SendBuffer<T>
    {
        private readonly ConcurrentQueue<T> buffer = new ConcurrentQueue<T>();
        private readonly Func<T, Task> send;
        private readonly int maxSize;
        private readonly Action<T> onDroppedLine;

        private object _lock = new object();
        private bool isRequestedNewLoop = true;
        private bool hasDroppedItems = false;
        


        public int Size => buffer.Count;


        public SendBuffer(Func<T, Task> send, Action<T> onDroppedLine,  int maxSize = -1)
        {
            this.send = send;
            this.maxSize = maxSize;
            this.onDroppedLine = onDroppedLine;
        }

        public void Enqueue(T line)
        {
            buffer.Enqueue(line);

            if (maxSize > 0 && buffer.Count > maxSize)
            {
                if (buffer.TryDequeue(out var droppedLine))
                    onDroppedLine(droppedLine);

                hasDroppedItems = true;
            }

            if (isRequestedNewLoop)
                Task.Run(() => SendLoop());
        }

        public bool WasDroppedItems()
        {
            var val = hasDroppedItems;
            hasDroppedItems = false;
            return val;
        }


        private async Task SendLoop()
        {
            while (true)
            {

                lock (_lock)
                {
                    if (!isRequestedNewLoop)
                        return;

                    isRequestedNewLoop = false;
                }


                //System.Diagnostics.Debug.WriteLine("!!!Run");

                while (buffer.TryDequeue(out var line))
                    await send(line);

                lock (_lock)
                {
                    isRequestedNewLoop = true;
                }

                if (buffer.IsEmpty)
                    break;
            }
        }

    }
}
