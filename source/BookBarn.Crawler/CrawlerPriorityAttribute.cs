namespace BookBarn.Crawler
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CrawlerPriorityAttribute : Attribute
    {
        public CrawlerPriorityAttribute(int priority)
        {
            Priority = priority;
            RetryWeight = 1;
        }

        /// <summary>
        /// Gets the crawler priority value.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Gets or sets the weight multiplier to apply on Priority when retrying a crawler.
        /// </summary>
        public double RetryWeight { get; set; }
    }
}
