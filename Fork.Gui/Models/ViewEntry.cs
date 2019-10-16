using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fork.Gui.Models
{
    public class ViewEntry : INotifyPropertyChanged
    {
        public string Kind { set; get; }
        public string Address { set; get; }
        public bool State { set; get; }
        public string BufferState { set; get; }
        public long TransferedCount { set; get; }
        public bool IsActive { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;


        public void Update(long transferredCount, bool isActive)
        {
            if (TransferedCount != transferredCount)
            {
                TransferedCount = transferredCount;
                Notify(nameof(TransferedCount));
            }

            if (IsActive != isActive)
            {
                IsActive = isActive;
                Notify(nameof(IsActive));
            }
        }

        public void Update(bool state, string bufferState, long transferredCount, bool isActive)
        {
            if (State != state)
            {
                State = state;
                Notify(nameof(State));
            }

            if (bufferState != BufferState)
            {
                BufferState = bufferState;
                Notify(nameof(BufferState));

            }

            Update(transferredCount, isActive);

        }

        private void Notify(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        //public void Notify()
        //{
        //    var t = PropertyChanged;

        //    if (t != null)
        //    {
        //        t(this, new PropertyChangedEventArgs(nameof(State)));
        //        t(this, new PropertyChangedEventArgs(nameof(BufferState)));
        //    }
        //}
    }
}
