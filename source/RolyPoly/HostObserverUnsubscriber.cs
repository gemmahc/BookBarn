using System.Collections.Concurrent;

namespace RolyPoly
{
    /// <summary>
    /// Exposes unsubscription logic for subscribers of CrawlerHost cycle events.
    /// </summary>
    public class HostObserverUnsubscriber : IDisposable
    {
        private ConcurrentDictionary<int, IObserver<HostInfo>> _observers;
        private int _observerIdx;

        /// <summary>
        /// Creates an unsubscriber instance removing the specified indexed observer from the provider's observers.
        /// </summary>
        /// <param name="observers">The dictionary instance containing the provider's observers</param>
        /// <param name="observerIdx">The index of the oberserver to remove on disposal</param>
        public HostObserverUnsubscriber(ConcurrentDictionary<int, IObserver<HostInfo>> observers, int observerIdx)
        {
            _observers = observers;
            _observerIdx = observerIdx;
        }

        /// <summary>
        /// Removes the subscription from the provider.
        /// </summary>
        public void Dispose()
        {
            _observers.TryRemove(_observerIdx, out _);
        }
    }
}
