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

        public BooksController(IBookDataProvider bookDataProvider)
        {
            _core = new BooksControllerCore(bookDataProvider);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> Get([FromQuery] int count, [FromQuery] string? afterId)
        {
            try
            {
                var result = await _core.Get(count, afterId);
                return Ok(result);
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Get(string id)
        {
            try
            {
                var result = await _core.Get(id);
                return Ok(result);
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Book>> Post([FromBody] Book book)
        {
            try
            {
                Book created = await _core.Post(book);
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex);
            }

            var request = this.HttpContext.Request;
            UriBuilder uriBuidler = new UriBuilder()
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Port = request.Host.Port.HasValue ? request.Host.Port.Value : default(int),
                Path = $"{request.Path}/{book.Id}"
            };

            return Created(uriBuidler.Uri, book);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> Put(string id, [FromBody] Book book)
        {
            try
            {
                var result = await _core.Put(id, book);
                return Ok(result);
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await _core.Delete(id);
                return NoContent();
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex);
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<IEnumerable<Book>>> Query([FromBody] BookQuery query)
        {
            try
            {
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
                return this.ResultFromDataError(ex);
            }
        }
    }
}
