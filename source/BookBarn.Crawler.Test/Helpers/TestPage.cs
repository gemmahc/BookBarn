using BookBarn.Crawler.Utilities;
using HtmlAgilityPack;

namespace BookBarn.Crawler.Test.Helpers
{
    internal class TestPage : Page<TestItem>
    {
        public TestPage(Uri endpoint, IPageClient client, IRequestThrottle throttle) : base(endpoint, client, throttle)
        {
        }

        protected override Task<TestItem> ExtractCore(HtmlDocument doc)
        {
            if(Action != null)
            {
                return Task.FromResult(Action.Invoke(doc));
            }

            return Task.FromResult(new TestItem());
        }

        public Func<HtmlDocument, TestItem>? Action { get; set; }
    }
}
