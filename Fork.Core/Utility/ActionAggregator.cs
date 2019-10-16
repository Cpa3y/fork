using Fork.Core.Destinations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Core.Utility
{
    public class ActionAggregator<T>
    {
        private readonly object _lock = new object();

        private readonly Func<List<T>, Task> queryExecutor;

        private readonly AtomicRemoveList<T> queries = new AtomicRemoveList<T>();

        private bool isRequestedNewQuery = true;


        public ActionAggregator(Func<List<T>, Task> queryExecutor)
        {
            this.queryExecutor = queryExecutor;
        }

        public ActionAggregator(Action<List<T>> queryExecutor): this(x => { queryExecutor(x); return Task.CompletedTask; })
        {

        }


        public void Queue(T query)
        {
            queries.Add(query);

            if (isRequestedNewQuery)
                Task.Run(() => RunQuery());
        }

        private async Task RunQuery()
        {
            while (true)
            {
                if (!HandleQueryRequest())
                    return;

                var pendingQueries = queries.Empty();

                if (pendingQueries.Count > 0)
                    await RunExecutor(pendingQueries);

                RequestNewQuery();

                if (pendingQueries.Count == 0)
                    break;
            }
        }


        private async Task RunExecutor(List<T> items)
        {
            try
            {
                await queryExecutor(items);
            }
            catch
            {

            }
        }

        private bool HandleQueryRequest()
        {
            lock (_lock)
            {
                if (!isRequestedNewQuery)
                    return false;

                isRequestedNewQuery = false;
                return true;
            }
        }

        private void RequestNewQuery()
        {
            lock (_lock)
                isRequestedNewQuery = true;
        }

    }
}
