using BookBarn.Crawler.Utilities;
using HtmlAgilityPack;

namespace BookBarn.Crawler
{
    public abstract class Page<T>
    {
        protected IRequestThrottle? _throttle;
        private HtmlDocument? _document;

        protected Page(Uri endpoint, IRequestThrottle throttle)
        {
            Endpoint = endpoint;
            _throttle = throttle;
        }

        public Page(Uri endpoint, HtmlDocument document)
        {
            _document = document;
            Initialized = true;
            Endpoint = endpoint;
        }

        public async Task<T> Extract()
        {
            if (!Initialized || _document == null)
            {
                await Init(_throttle);
            }

            if (_document == null)
            {
                throw new PageParseException(Endpoint, $"Failed to load the page at [{Endpoint}]");
            }

            return await ExtractCore(_document);
        }


        protected abstract Task<T> ExtractCore(HtmlDocument doc);


        protected async Task Init(IRequestThrottle? throttle)
        {
            ArgumentNullException.ThrowIfNull(throttle);

            _document = await throttle.RunAsync(Endpoint, async (target) =>
            {
                var web = new HtmlWeb();
                return await web.LoadFromWebAsync(target.ToString());
            });

            Initialized = true;
        }

        public bool Initialized { get; private set; }

        public Uri Endpoint { get; private set; }
    }
}
