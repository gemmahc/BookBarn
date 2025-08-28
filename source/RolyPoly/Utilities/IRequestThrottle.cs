namespace BookBarn.Crawler.Utilities
{
    public interface IRequestThrottle
    {
        /// <summary>
        /// Executes a unit of work against the specified endpoint, enforcing implemented throttle rules.
        /// </summary>
        /// <typeparam name="T">The return type of the request.</typeparam>
        /// <param name="partitionKey">The key to the partition to throttle.</param>
        /// <param name="unitOfWork">The delegate that runs the request against the endpoint.</param>
        /// <returns>The result of the request.</returns>
        public Task<T> RunAsync<T>(Uri endpoint, Func<Uri, Task<T>> unitOfWork);
    }
}
