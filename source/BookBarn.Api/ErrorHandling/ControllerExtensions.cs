using Microsoft.AspNetCore.Mvc;

namespace BookBarn.Api.ErrorHandling
{
    public static class ControllerExtensions
    {
        public static StatusCodeResult ResultFromDataError(this ControllerBase controller, DataException ex)
        {
            switch (ex.Error)
            {
                case DataError.AlreadyExists:
                    return controller.Conflict();
                case DataError.NotFound:
                    return controller.NotFound();
                default:
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
