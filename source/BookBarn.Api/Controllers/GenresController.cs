using BookBarn.Model;
using BookBarn.Model.Providers;
using Microsoft.AspNetCore.Mvc;
using BookBarn.Api.ErrorHandling;

namespace BookBarn.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private GenresControllerCore _core;
        private ILogger _logger;

        public GenresController(IGenreDataProvider genreProvider, ILogger<GenresController> logger)
        {
            _logger = logger;
            _core = new GenresControllerCore(genreProvider, _logger);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> Get()
        {
            try
            {
                _logger.LogInformation("Getting all genres");
                var result = await _core.Get();
                return Ok(result);
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get the genre list");
                throw;
            }
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Genre>> Get(string name)
        {
            try
            {
                _logger.LogInformation("Getting genre information for [{genre}]", name);
                var result = await _core.Get(name);
                return Ok(result);
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get genre information for [{genre}]", name);
                throw;
            }
        }
    }
}
