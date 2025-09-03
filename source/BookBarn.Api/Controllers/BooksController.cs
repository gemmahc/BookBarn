using BookBarn.Model;
using BookBarn.Model.Providers;
using Microsoft.AspNetCore.Mvc;
using BookBarn.Api.ErrorHandling;

namespace BookBarn.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private BooksControllerCore _core;
        private ILogger _logger;

        public BooksController(IBookDataProvider bookDataProvider, ILogger<BooksController> logger)
        {
            _logger = logger;

            _core = new BooksControllerCore(bookDataProvider, _logger);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> Get([FromQuery] int count, [FromQuery] string? afterId)
        {
            try
            {
                _logger.LogInformation("Getting [{count}] books starting after id [{id}]", count, afterId);
                var result = await _core.Get(count, afterId);
                return Ok(result);
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to return range result count [{count}] after id [{afterId}]", count, afterId);
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Get(string id)
        {
            try
            {
                _logger.LogInformation("Getting book with id [{id}]", id);
                var result = await _core.Get(id);
                return Ok(result);
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get failed for [{id}]", id);
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult<Book>> Post([FromBody] Book book)
        {
            try
            {
                _logger.LogInformation("Adding book with id [{id}]", book.Id);
                Book created = await _core.Post(book);
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to post book with id [{id}]", book.Id);
                throw;
            }

            var request = this.HttpContext.Request;
            UriBuilder uriBuidler = new UriBuilder()
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Port = request.Host.Port.HasValue ? request.Host.Port.Value : default(int),
                Path = $"{request.Path}/{book.Id}"
            };

            _logger.LogInformation("Created book resource [{id}] at [{uri}]", book.Id, uriBuidler.Uri);
            return Created(uriBuidler.Uri, book);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> Put(string id, [FromBody] Book book)
        {
            try
            {
                _logger.LogInformation("Updating book with id [{id}]", id);
                var result = await _core.Put(id, book);
                return Ok(result);
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to put resource [{id}]", id);
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                _logger.LogInformation("Deleting book with id [{id}]", id);
                await _core.Delete(id);
                return NoContent();
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete resource [{id}]", id);
                throw;
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<IEnumerable<Book>>> Query([FromBody] BookQuery query)
        {
            try
            {
                _logger.LogInformation("Executing query: {query}", query.ToJson());
                var result = await _core.Query(query);

                if (result.Any())
                {
                    return Ok(result);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute query");
                throw;
            }
        }
    }
}
