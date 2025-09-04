using Microsoft.Extensions.Options;

namespace BookBarn.Crawler.Host
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptions<CrawlerHostConfiguration> _hostConfig;
        private readonly ICrawlerFactory _crawlerFactory;
        private readonly ICrawlerQueue _queue;

        public Worker(IOptions<CrawlerHostConfiguration> hostConfig, ILogger<Worker> logger, ICrawlerFactory crawlerFactory, ICrawlerQueue queue)
        {
            _logger = logger;
            _hostConfig = hostConfig;
            _crawlerFactory = crawlerFactory;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CrawlerHost host = new CrawlerHost(_hostConfig.Value, _crawlerFactory, _queue, _logger);

            _logger.LogInformation("Starting crawler host.");
            try
            {
                await host.RunAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The crawler host stopped unexpectedly");
                throw;
            }
            finally
            {
                _logger.LogInformation("Crawler host stopped.");
            }
        }
    }
}
