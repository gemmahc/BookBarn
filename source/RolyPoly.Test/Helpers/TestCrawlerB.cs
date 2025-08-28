namespace RolyPoly.Test
{
    public class TestCrawlerB : Crawler
    {
        public TestCrawlerB(Uri entrypoint) : base(entrypoint)
        {
            CrawlerAction = new Action(() => { });
            ChildrenToAdd = new List<string>();
        }

        protected override Task RunCrawlerAsync()
        {
            foreach (var link in ChildrenToAdd)
            {
                DispatchChild<TestCrawlerB>(new Uri(link));
            }
            return Task.Run(CrawlerAction);
        }

        public Action CrawlerAction { get; set; }

        /// <summary>
        /// Adds children with type TestCrawlerB
        /// </summary>
        public List<string> ChildrenToAdd { get; set; }
    }
}
