using BookBarn.Crawler.GoodReads;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace BookBarn.Crawler.Host
{
    public class InMemoryQueue : ICrawlerQueue
    {
        object _lock = new object();

        public InMemoryQueue()
        {
        }

        public Task Enqueue(CrawlRequest request)
        {
            CrawlerPriorityAttribute? priorityAttr = Attribute.GetCustomAttribute(request.RequestedCrawler, typeof(CrawlerPriorityAttribute)) as CrawlerPriorityAttribute;

            int priority = priorityAttr?.Priority ?? 1;

            lock (_lock)
            {
                var queue = GetQueue();
                queue?.Enqueue(request, priority);
                SetQueue(queue);
            }

            return Task.CompletedTask;
        }

        public Task<CrawlRequest?> GetNext()
        {
            CrawlRequest? request = null;

            lock (_lock)
            {
                var queue = GetQueue();
                if (queue?.Count > 0)
                {
                    request = queue.Dequeue();
                }
                SetQueue(queue);
            }

            return Task.FromResult(request);
        }

        public Task<bool> HasWork()
        {
            lock (_lock)
            {
                var queue = GetQueue();
                return Task.FromResult(queue?.Count > 0);
            }
        }

        private PriorityQueue<CrawlRequest, int>? GetQueue()
        {
            var queue = new PriorityQueue<CrawlRequest, int>();

            if(!File.Exists("Queue.txt"))
            {
                return queue;
            }

            string[] queueTxt = File.ReadAllLines("Queue.txt");
            foreach (string line in queueTxt)
            {
                var seg = line.Split('`');
                Uri ep = new Uri(seg[0]);
                Type? t = Type.GetType(seg[1]) ?? typeof(Crawler);
                int p = int.Parse(seg[2]);

                queue.Enqueue(new CrawlRequest(ep, t), p);
            }

            return queue;
        }

        private void SetQueue(PriorityQueue<CrawlRequest, int>? queue)
        {
            if (queue == null)
            {
                return;
            }
            List<string> lines = new List<string>();
            while(queue.Count > 0)
            {
                var item = queue.Dequeue();
                CrawlerPriorityAttribute? priorityAttr = Attribute.GetCustomAttribute(item.RequestedCrawler, typeof(CrawlerPriorityAttribute)) as CrawlerPriorityAttribute;
                int priority = priorityAttr?.Priority ?? 1;
                string line = $"{item.Endpoint}`{item.RequestedCrawler.AssemblyQualifiedName}`{priority}";
                lines.Add(line);
            }

            File.WriteAllLines("Queue.txt", lines);
        }
    }
}
