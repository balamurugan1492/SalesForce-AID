#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

#region Pointel Namespace
using Pointel.Statistics.Core.StatisticsProvider;
#endregion

namespace Pointel.Statistics.Core.Provider
{
    /// <summary>
    /// StatisticsDataProvider 
    /// </summary>
    internal class StatisticsDataProvider : IObservable<IStatisticsCollection>
    {
        private List<IObserver<IStatisticsCollection>> observers;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsDataProvider" /> class.
        /// </summary>
        public StatisticsDataProvider()
        {
            observers = new List<IObserver<IStatisticsCollection>>();
        }

        /// <summary>
        /// Subscribes the specified observer.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<IStatisticsCollection> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
            return new Unsubscriber<IStatisticsCollection>(observers, observer);
        }

        /// <summary>
        /// News the statistics data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void NewStatisticsData(IStatisticsCollection data)
        {
            foreach (var observer in observers)
            {
                observer.OnNext(data);
            }
        }

        /// <summary>
        /// Closes the integration.
        /// </summary>
        public void CloseIntegration()
        {
            foreach (var observer in observers)
            {
                observer.OnCompleted();

                observers.Clear();
            }
        }
    }
}
