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

        public GenresController(IGenreDataProvider genreProvider)
        {
            _core = new GenresControllerCore(genreProvider);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> Get()
        {
            try
            {
                var result = await _core.Get();
                return Ok(result);
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex);
            }
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Genre>> Get(string name)
        {
            try
            {
                var result = await _core.Get(name);
                return Ok(result);
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex);
            }
        }
    }
}
