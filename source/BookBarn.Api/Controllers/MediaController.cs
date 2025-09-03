using BookBarn.Api.ErrorHandling;
using BookBarn.Model;
using BookBarn.Model.Providers;
using Microsoft.AspNetCore.Mvc;

namespace BookBarn.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private MediaControllerCore _core;
        private ILogger _logger;

        public MediaController(IMediaStorageProvider storageProvider, ILogger<MediaController> logger)
        {
            _logger = logger;
            _core = new MediaControllerCore(storageProvider, _logger);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                _logger.LogInformation("Deleting media with id [{id}]", id);
                await _core.Delete(id);

                return NoContent();

            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed deleting media with id [{id}]", id);
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Media>> Get(string id)
        {
            try
            {
                _logger.LogInformation("Getting media with id [{id}]", id);
                var result = await _core.Get(id);

                return Ok(result);

            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get media with id [{id}]", id);
                throw;
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<ActionResult<MediaStorageToken>> GetWriteToken(string id)
        {
            try
            {
                MediaStorageToken token = await _core.GetWriteToken(id);

                return Ok(token);
            }
            catch (DataException ex)
            {
                return this.ResultFromDataError(ex, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get write token for media with id [{id}]", id);
                throw;
            }
        }

    }
}
