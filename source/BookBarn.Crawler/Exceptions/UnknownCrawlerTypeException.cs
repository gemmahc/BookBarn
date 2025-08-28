namespace BookBarn.Crawler
{
    public class UnknownCrawlerTypeException : Exception
    {
        public UnknownCrawlerTypeException(Type t)
        {
            Type = t;
        }

        public UnknownCrawlerTypeException(Type t, string? message) : base(message)
        {
            Type = t;
        }

        public UnknownCrawlerTypeException(Type t, string? message, Exception? innerException) : base(message, innerException)
        {
            Type = t;
        }

        public Type Type { get; set; }
    }
}
