using BookBarn.Api.ErrorHandling;
using BookBarn.Api.v1;
using BookBarn.Model;
using BookBarn.Model.Providers;

namespace BookBarn.Api.Controllers
{
    /// <summary>
    /// Implementation of the IBooksController interface.
    /// BooksController handles necessary Http request/response items but
    /// is otherwise a passthrough to this class
    /// </summary>
    public class BooksControllerCore : IBooksService
    {
        private IBookDataProvider _dataProvider;

        public BooksControllerCore(IBookDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task Delete(string id)
        {
            if (!await _dataProvider.Delete(id))
            {
                throw new DataException(DataError.General);
            }
        }

        public async Task<IEnumerable<Book>> Get(int count, string? afterId)
        {
            var result = await _dataProvider.GetMany(count, afterId);

            if (result == null)
            {
                throw new DataException(DataError.NotFound);
            }

            return result;
        }

        public async Task<Book> Get(string id)
        {
            var result = await _dataProvider.Get(id);

            if (result == null)
            {
                throw new DataException(DataError.NotFound);
            }

            return result;
        }

        public async Task<Book> Post(Book book)
        {
            string id = book.GetId();

            Book existing = await _dataProvider.Get(id);

            if (existing != null)
            {
                throw new DataException(DataError.AlreadyExists);
            }

            book.Id = id;

            var result = await _dataProvider.Upsert(book);

            if (result == null)
            {
                throw new DataException(DataError.General);
            }

            return result;
        }

        public async Task<Book> Put(string id, Book book)
        {
            // For now just add or replace the whole record. In the future add logic for differential update.

            book.Id = id;

            Book updated = await _dataProvider.Upsert(book);

            if(updated == null)
            {
                throw new DataException(DataError.General);
            }

            return updated;
        }

        public async Task<IEnumerable<Book>> Query(BookQuery query)
        {
            // ToDo: implement pagination/partial results

            var result = await _dataProvider.QueryBooks(query);

            if (result == null)
            {
                // Query failed to run.
                throw new DataException(DataError.General);
            }

            return result;
        }
    }
}
