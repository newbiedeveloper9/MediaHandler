using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace MediaHandler.ViewModels
{
    public partial class ThreadViewModel : IDeactivate,
        IDisposable
    {
        bool disposed = false;

        #region CloseWindow
        public void Deactivate(bool close)
        {
            Dispose();
        }

        public event EventHandler<DeactivationEventArgs> AttemptingDeactivation;
        public event EventHandler<DeactivationEventArgs> Deactivated;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _eventAggregator.Unsubscribe(this);
                _eventAggregator = null;
                FbThreadService.Thread = null;
                FbThreadService = null;
                _scrollViewerLogic = null;
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ThreadViewModel()
        {
            Dispose(false);
        }
        #endregion CloseWindow
    }
}
