using BookBarn.Crawler.Utilities;

namespace BookBarn.Crawler.GoodReads.Test
{
    internal static class Utilities
    {
        private static RequestThrottle? _testThrottle;

        internal static RequestThrottle GetThrottle()
        {
            if (_testThrottle == null)
            {
                RequestThrottleOptions options = new RequestThrottleOptions()
                {
                    Interval = TimeSpan.FromSeconds(1),
                    MaxConcurrentRequests = 2,
                    MaxQueuedRequests = 1000
                };
                _testThrottle = new RequestThrottle(options);
            }

            return _testThrottle;
        }
    }
}
