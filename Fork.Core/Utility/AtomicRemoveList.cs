using System;
using System.Collections.Generic;
using System.Text;

namespace Fork.Core.Utility
{

    // TODO: Написать более эффективный вариант (возможно readerwriterlock)
    public class AtomicRemoveList<T>
    {
        private readonly object _lock = new object();
        private List<T> list = new List<T>();
             

        public void Add(T item)
        {
            lock(_lock)
            {
                list.Add(item);
            }
        }

        public List<T> Empty()
        {
            lock(_lock)
            {
                var result = list;
                list = new List<T>();
                return result;
            }
        }
    }
}
