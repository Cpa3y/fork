using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Core.Connections
{
    public class ListenerHelper<T> where T:class, IDisposable
    { 
        private readonly Func<Task<T>> factory;
        private readonly Dispenser<T> dispenser;


            
        public ListenerHelper(Func<Task<T>> factory)
        {
            this.factory = factory;
            this.dispenser = new Dispenser<T>();
        }

        public void Start()
        {

            Task.Run(() => Loop());
        }

        public Task<T> Request() => dispenser.Get();

        private async Task Loop()
        {
            T con = null;
            while (true)
            {
                var newConnection = await factory();
                con?.Dispose();
                con = newConnection;
                dispenser.Put(newConnection);
            }
        }
    }
}
