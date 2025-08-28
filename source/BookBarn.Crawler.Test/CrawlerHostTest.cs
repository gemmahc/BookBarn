namespace BookBarn.Crawler.Test
{
    public class CrawlerHostTest
    {
        [Fact]
        public async Task HostStartsCyclesAndStops()
        {
            var queue = new TestCrawlerQueue();
            var factory = new TestCrawlerFactory();
            var config = GetConfig();

            CrawlerHost host = new CrawlerHost(config, factory, queue);

            using (CancellationTokenSource source = new CancellationTokenSource())
            {

                HostObserver obs = new HostObserver();
                obs.Asserts = (hi) =>
                {
                    // Wait for it to cycle a second time.
                    if (hi.Cycle < 2)
                    {
                        return;
                    }

                    Assert.Equal(2, hi.Cycle);
                    Assert.True(hi.HostIdle);

                    source.Cancel();
                };

                host.Subscribe(obs);

                await host.RunAsync(source.Token);
            }
        }

        [Fact]
        public async Task HostStartsAndExecutesCrawler()
        {
            var queue = new TestCrawlerQueue();
            var factory = new TestCrawlerFactory();
            var config = GetConfig();
            Uri endpoint = new Uri("https://github.com/gemmahc/BookBarn");
            CrawlerHost host = new CrawlerHost(config, factory, queue);

            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                HostObserver obs = new HostObserver();
                obs.Asserts = (hi) =>
                {
                    // Queue for the second cycle
                    if ((hi.Cycle == 1))
                    {
                        queue.Enqueue(new CrawlRequest(endpoint, typeof(TestCrawlerA)));
                        return;
                    }

                    // Second cycle dequeued, executed, processed result, went idle
                    if (hi.Cycle == 2)
                    {
                        Assert.True(hi.HostIdle);
                        Assert.Single(hi.SucceededLastCycle);
                        Assert.Equal(endpoint, hi.SucceededLastCycle[0]);
                        Assert.Empty(hi.CurrentlyRunning);
                    }

                    source.Cancel();
                };

                host.Subscribe(obs);

                await host.RunAsync(source.Token);
            }
        }

        [Fact]
        public async Task HostStartsMultipleAndExecutesCrawlers()
        {
            var queue = new TestCrawlerQueue();
            var factory = new TestCrawlerFactory();
            var config = GetConfig();
            Uri endpoint = new Uri("https://github.com/gemmahc/BookBarn");
            CrawlerHost host = new CrawlerHost(config, factory, queue);

            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                HostObserver obs = new HostObserver();
                obs.Asserts = (hi) =>
                {
                    // Queue for the second cycle
                    if ((hi.Cycle == 1))
                    {
                        queue.Enqueue(new CrawlRequest(new Uri(endpoint, "/1"), typeof(TestCrawlerA)));
                        queue.Enqueue(new CrawlRequest(new Uri(endpoint, "/2"), typeof(TestCrawlerA)));
                        queue.Enqueue(new CrawlRequest(new Uri(endpoint, "/3"), typeof(TestCrawlerA)));

                        return;
                    }

                    // Second cycle dequeued /1, executed, processed result
                    if (hi.Cycle == 2)
                    {
                        Assert.False(hi.HostIdle);
                        Assert.Single(hi.SucceededLastCycle);
                        Assert.Equal(new Uri(endpoint, "/1"), hi.SucceededLastCycle[0]);
                        Assert.Empty(hi.CurrentlyRunning);
                        return;
                    }

                    // Third cycle dequeued /2, executed, processed result
                    if (hi.Cycle == 3)
                    {
                        Assert.False(hi.HostIdle);
                        Assert.Single(hi.SucceededLastCycle);
                        Assert.Equal(new Uri(endpoint, "/2"), hi.SucceededLastCycle[0]);
                        Assert.Empty(hi.CurrentlyRunning);
                        return;
                    }

                    // Fourth cycle dequeued /3, executed, processed result, went idle.
                    if (hi.Cycle == 4)
                    {
                        Assert.True(hi.HostIdle);
                        Assert.Single(hi.SucceededLastCycle);
                        Assert.Equal(new Uri(endpoint, "/3"), hi.SucceededLastCycle[0]);
                        Assert.Empty(hi.CurrentlyRunning);
                        return;
                    }

                    if (hi.Cycle > 4)
                    {
                        // Queue is empty, ensure idle and cancel the host runner.
                        Assert.True(hi.HostIdle);
                        source.Cancel();
                    }
                };

                host.Subscribe(obs);

                await host.RunAsync(source.Token);
            }
        }

        [Fact]
        public async Task HostSkipsDuplicateCrawlers()
        {
            var queue = new TestCrawlerQueue();
            var factory = new TestCrawlerFactory();
            var config = GetConfig();
            Uri endpoint = new Uri("https://github.com/gemmahc/BookBarn");
            CrawlerHost host = new CrawlerHost(config, factory, queue);

            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                HostObserver obs = new HostObserver();
                obs.Asserts = (hi) =>
                {
                    // Queue for the second cycle
                    if ((hi.Cycle == 1))
                    {
                        queue.Enqueue(new CrawlRequest(endpoint, typeof(TestCrawlerA)));
                        queue.Enqueue(new CrawlRequest(endpoint, typeof(TestCrawlerA)));

                        return;
                    }

                    // Second cycle dequeued first, executed, processed result
                    if (hi.Cycle == 2)
                    {
                        Assert.False(hi.HostIdle);
                        Assert.Single(hi.SucceededLastCycle);
                        Assert.Equal(endpoint, hi.SucceededLastCycle[0]);
                        Assert.Empty(hi.CurrentlyRunning);
                        return;
                    }

                    // Third cycle dequeued duplicate, skipped goes idle.
                    if (hi.Cycle == 3)
                    {
                        Assert.True(hi.HostIdle);
                        Assert.Single(hi.DuplicatesSkippedLastCycle);
                        Assert.Equal(endpoint, hi.DuplicatesSkippedLastCycle[0]);
                        Assert.Empty(hi.CurrentlyRunning);
                        return;
                    }

                    if (hi.Cycle > 3)
                    {
                        // Queue is empty, ensure idle and cancel the host runner.
                        Assert.True(hi.HostIdle);
                        source.Cancel();
                    }
                };

                host.Subscribe(obs);

                await host.RunAsync(source.Token);
            }
        }


        [Fact]
        public async Task HostStartsFailsAndRetriesCrawler()
        {
            var queue = new TestCrawlerQueue();
            var factory = new TestCrawlerFactory();
            var config = GetConfig();
            config.FailedCrawlerRetryCount = 1;
            Uri endpoint = new Uri("https://github.com/gemmahc/BookBarn");
            CrawlerHost host = new CrawlerHost(config, factory, queue);

            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                HostObserver obs = new HostObserver();
                obs.Asserts = (hi) =>
                {
                    // Queue crawler for the next cycle
                    if ((hi.Cycle == 1))
                    {
                        // set crawler factory to generate a failed crawler.
                        factory.CrawlerAConstructor = new Func<Uri, TestCrawlerA>((ep) =>
                        {
                            var tca = new TestCrawlerA(ep);
                            tca.CrawlerAction = new Action(() => { throw new Exception("Fails!"); });
                            return tca;
                        });

                        queue.Enqueue(new CrawlRequest(endpoint, typeof(TestCrawlerA)));

                        return;
                    }

                    // Second cycle dequeued, executed, processed failed result, queued retry
                    if (hi.Cycle == 2)
                    {
                        Assert.False(hi.HostIdle);
                        Assert.Single(hi.FailedLastCycle);
                        Assert.Equal(endpoint, hi.FailedLastCycle[0]);
                        Assert.Single(hi.RetriesQueuedLastCycle);
                        Assert.Equal(endpoint, hi.RetriesQueuedLastCycle[0]);
                        Assert.Empty(hi.CurrentlyRunning);

                        // set crawler factory to generate a successful crawler.
                        factory.CrawlerAConstructor = new Func<Uri, TestCrawlerA>((ep) =>
                        {
                            return new TestCrawlerA(ep);
                        });

                        return;
                    }

                    // Third cycle dequeued, executed, preocessed success result, went idle
                    if (hi.Cycle == 3)
                    {
                        Assert.True(hi.HostIdle);

                        Assert.Single(hi.SucceededLastCycle);
                        Assert.Equal(endpoint, hi.SucceededLastCycle[0]);
                        Assert.Empty(hi.CurrentlyRunning);

                        return;
                    }

                    if (hi.Cycle > 3)
                    {
                        // Queue is empty, ensure idle and cancel the host runner.
                        Assert.True(hi.HostIdle);
                        source.Cancel();
                    }
                };

                host.Subscribe(obs);

                await host.RunAsync(source.Token);
            }
        }

        [Fact]
        public async Task HostRunsChildCrawlers()
        {
            var queue = new TestCrawlerQueue();
            var factory = new TestCrawlerFactory();
            var config = GetConfig();
            Uri endpoint = new Uri("https://github.com/gemmahc/BookBarn");
            Uri childEndpoint = new Uri(endpoint, "/child");
            CrawlerHost host = new CrawlerHost(config, factory, queue);

            using (CancellationTokenSource source = new CancellationTokenSource())
            {
                HostObserver obs = new HostObserver();
                obs.Asserts = (hi) =>
                {
                    // Queue crawler for the next cycle
                    if ((hi.Cycle == 1))
                    {
                        // set crawler factory to generate a crawler with children (duplicates)
                        factory.CrawlerBConstructor = new Func<Uri, TestCrawlerB>((ep) =>
                        {
                            var tcb = new TestCrawlerB(ep);
                            tcb.ChildrenToAdd.Add(childEndpoint.ToString()); // child
                            tcb.ChildrenToAdd.Add(endpoint.ToString()); // duplicate of parent
                            return tcb;
                        });

                        queue.Enqueue(new CrawlRequest(endpoint, typeof(TestCrawlerB)));

                        return;
                    }

                    // Second cycle dequeued, executed, processed result, 1st child queued, 2nd child skipped
                    if (hi.Cycle == 2)
                    {
                        Assert.False(hi.HostIdle);
                        Assert.Single(hi.SucceededLastCycle);
                        Assert.Equal(endpoint, hi.SucceededLastCycle[0]);
                        Assert.Equal(2, hi.ChildrenProcessedLastCycle.Count);
                        Assert.Equal(childEndpoint, hi.ChildrenProcessedLastCycle[0]);
                        Assert.Equal(endpoint, hi.ChildrenProcessedLastCycle[1]);
                        Assert.Single(hi.DuplicatesSkippedLastCycle);
                        Assert.Equal(endpoint, hi.DuplicatesSkippedLastCycle[0]);
                        Assert.Empty(hi.CurrentlyRunning);

                        // set crawler factory to generate a crawler without a child.
                        factory.CrawlerBConstructor = new Func<Uri, TestCrawlerB>((ep) =>
                        {
                            return new TestCrawlerB(ep);
                        });

                        return;
                    }

                    // Third cycle child dequeued, executed, preocessed success result, went idle
                    if (hi.Cycle == 3)
                    {
                        Assert.True(hi.HostIdle);

                        Assert.Single(hi.SucceededLastCycle);
                        Assert.Equal(childEndpoint, hi.SucceededLastCycle[0]);
                        Assert.Empty(hi.CurrentlyRunning);

                        return;
                    }

                    if (hi.Cycle > 3)
                    {
                        // Queue is empty, ensure idle and cancel the host runner.
                        Assert.True(hi.HostIdle);
                        source.Cancel();
                    }
                };

                host.Subscribe(obs);

                await host.RunAsync(source.Token);
            }
        }


        private CrawlerHostConfiguration GetConfig()
        {
            return new CrawlerHostConfiguration()
            {
                MaxConcurrentCrawlers = 1,
                FailedCrawlerRetryCount = 0,
                IdlePollingIntervalMilliseconds = 1
            };
        }
    }
}
