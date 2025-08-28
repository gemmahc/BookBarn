namespace RolyPoly.Test
{
    public class TestCrawlerA : Crawler
    {
        public TestCrawlerA(Uri entrypoint) : base(entrypoint)
        {
            CrawlerAction = new Action(() => { });
            ChildrenToAdd = new List<string>();
        }

        protected override Task RunCrawlerAsync()
        {
            bool tick = false;
            foreach (string child in ChildrenToAdd)
            {
                if (tick)
                {
                    DispatchChild<TestCrawlerB>(new Uri(child));
                }
                else
                {
                    DispatchChild<TestCrawlerA>(new Uri(child));
                }

                tick = !tick;
            }

            return Task.Run(CrawlerAction);
        }

        public Action CrawlerAction { get; set; }

        public List<string> ChildrenToAdd { get; set; }
    }
}
