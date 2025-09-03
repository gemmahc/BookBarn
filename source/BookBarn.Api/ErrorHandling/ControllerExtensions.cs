using Microsoft.AspNetCore.Mvc;

namespace BookBarn.Api.ErrorHandling
{
    public static class ControllerExtensions
    {
        public static StatusCodeResult ResultFromDataError(this ControllerBase controller, DataException ex, ILogger logger)
        {
            switch (ex.Error)
            {
                case DataError.AlreadyExists:
                    logger.LogWarning(ex, "The record already exists.");
                    return controller.Conflict();
                case DataError.NotFound:
                    logger.LogWarning(ex, "The record was not found.");
                    return controller.NotFound();
                default:
                    logger.LogError(ex, "Generic data error");
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
