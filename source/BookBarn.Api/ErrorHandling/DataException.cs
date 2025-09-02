using BookBarn.Api.ErrorHandling;

namespace BookBarn.Api
{
    public class DataException : Exception
    {
        public DataException()
        {
        }

        public DataException(DataError error, string? message = null, Exception? innerException = null) : base(message, innerException)
        {
            Error = error;
        }

        public DataError Error { get; set; }
    }
}
