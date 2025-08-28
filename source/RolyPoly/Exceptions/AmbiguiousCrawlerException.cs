namespace BookBarn.Crawler
{
    internal class AmbiguiousCrawlerException : Exception
    {
        public AmbiguiousCrawlerException()
        {
        }

        public AmbiguiousCrawlerException(string? message) : base(message)
        {
        }
    }
}
