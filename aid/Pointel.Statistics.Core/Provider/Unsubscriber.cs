#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Pointel.Statistics.Core.Provider
{
    /// <summary>
    /// Unsubscriber
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Unsubscriber<T> : IDisposable
    {
        #region Field Declaration
        private List<IObserver<T>> _observers;
        private IObserver<T> _observer;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Unsubscriber&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="observers">The observers.</param>
        /// <param name="observer">The observer.</param>
        internal Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            this._observers=observers;
            this._observer=observer;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if(_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
