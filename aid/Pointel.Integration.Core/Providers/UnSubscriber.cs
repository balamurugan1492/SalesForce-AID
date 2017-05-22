#region System Namespaces

using System;
using System.Collections.Generic;

#endregion System Namespaces

namespace Pointel.Integration.Core.Providers
{
    internal class Unsubscriber<T> : IDisposable
    {
        #region Declarations

        private List<IObserver<T>> _observers;
        private IObserver<T> _observer;

        #endregion Declarations

        /// <summary>
        /// Initializes a new instance of the <see cref="Unsubscriber{T}"/> class.
        /// </summary>
        /// <param name="observers">The observers.</param>
        /// <param name="observer">The observer.</param>

        #region Unsubscriber

        internal Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        #endregion Unsubscriber

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>

        #region Dispose

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }

        #endregion Dispose
    }
}