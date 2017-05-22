#region System Namespace

using System;
using System.Collections.Generic;
using System.Linq;

#endregion System Namespace

#region AID Namespace

using Pointel.Integration.Core.iSubjects;

#endregion AID Namespace

namespace Pointel.Integration.Core.Providers
{
    internal class CallDataProviders : IObservable<iCallData>
    {
        #region Field Declaration

        private List<IObserver<iCallData>> observers;

        #endregion Field Declaration

        /// <summary>
        /// Initializes a new instance of the <see cref="CallDataProviders"/> class.
        /// </summary>

        #region CallDataProviders

        public CallDataProviders()
        {
            observers = new List<IObserver<iCallData>>();
        }

        #endregion CallDataProviders

        /// <summary>
        /// Subscribes the specified observer.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <returns></returns>

        #region Subscribe

        public IDisposable Subscribe(IObserver<iCallData> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
            return new Unsubscriber<iCallData>(observers, observer);
        }

        #endregion Subscribe

        public void NotifyLoginURL()
        {
            if (observers != null)
            {
                foreach (var observer in observers)
                    observer.OnNext(null);

            }
        }

        /// <summary>
        /// News the call data.
        /// </summary>
        /// <param name="data">The data.</param>

        #region NewCallData

        public void NewCallData(iCallData data)
        {
            foreach (var observer in observers)
                observer.OnNext(data);
        }

        #endregion NewCallData

        /// <summary>
        /// Closes the integration.
        /// </summary>

        #region CloseIntegration

        public void CloseIntegration()
        {
            foreach (var observer in observers)
                observer.OnCompleted();
            observers.Clear();
        }

        #endregion CloseIntegration

        public T GetObserver<T>()
        {
            return (T)observers.SingleOrDefault(x => x is T);
        }

        //public T GetObject<T>()
        //{
        //    //if(observers)
        //}
    }
}