using Microsoft.AspNetCore.Mvc;

namespace BookBarn.Crawler.Host.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class QueueController : Controller
    {
        private ICrawlerQueue _queue;

        public QueueController(ICrawlerQueue queue)
        {
            _queue = queue;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<CrawlRequest>> Dequeue()
        {
            var next = await _queue.GetNext();

            if (next == null)
            {
                return NoContent();
            }

            return Ok(next);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Enqueue([FromQuery] string target, [FromQuery] string crawlerType)
        {
            if (!Uri.TryCreate(target, UriKind.Absolute, out Uri? endpoint))
            {
                return BadRequest($"Specified target [{target}] is not a valid uri");
            }

            Type? type = Type.GetType(crawlerType);

            if (type == null || !typeof(Crawler).IsAssignableFrom(type))
            {
                return BadRequest($"Type [{crawlerType}] is not a valid crawler type");
            }

            await _queue.Enqueue(new CrawlRequest(endpoint, type));

            return Accepted();
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<int>> Count()
        {
            return Ok(await _queue.Count());
        }
    }
}
