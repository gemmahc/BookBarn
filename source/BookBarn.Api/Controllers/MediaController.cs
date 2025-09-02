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

        public MediaController(IMediaStorageProvider storageProvider)
        {
            _core = new MediaControllerCore(storageProvider);
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

        [HttpGet("{id}")]
        public async Task<ActionResult<Media>> Get(string id)
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


            throw new NotImplementedException();
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
                return this.ResultFromDataError(ex);
            }
        }

    }
}
