using BookBarn.Crawler.Utilities;

namespace BookBarn.Crawler.Test
{
    internal class Utilities
    {
        private static RequestThrottle? _testThrottle;

        internal static RequestThrottle GetThrottle()
        {
            if (_testThrottle == null)
            {
                RequestThrottleOptions options = new RequestThrottleOptions()
                {
                    IntervalSeconds = 1,
                    MaxConcurrentRequests = 2,
                    MaxQueuedRequests = 1000
                };
                _testThrottle = new RequestThrottle(options);
            }

            return _testThrottle;
        }
    }
}
