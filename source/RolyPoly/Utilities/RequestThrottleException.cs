namespace BookBarn.Crawler.Utilities
{
    /// <summary>
    /// Represents an error condition of a RequestThrottle
    /// </summary>
    public class RequestThrottleException : Exception
    {
        public RequestThrottleException()
        {
        }

        public RequestThrottleException(string? message) : base(message)
        {
        }
    }
}
