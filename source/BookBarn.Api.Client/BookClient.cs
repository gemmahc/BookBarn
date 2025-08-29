using BookBarn.Model;
using BookBarn.Model.API.v1;

namespace BookBarn.Api.Client
{
    public class BookClient : ApiClient, IBooksController
    {
        public BookClient(Uri endpoint) : base(new Uri(endpoint, "/v1/Books")) { }

        public BookClient(HttpClient client, Uri endpoint) : base(client, new Uri(endpoint, "/v1/Books")) { }

        public async Task Delete(string id)
        {
            await base.DeleteAsync(id);
        }

        public async Task<IEnumerable<Book>> Get(int count, string? afterId = null)
        {
            string queryString = $"count={count}";

            if (!string.IsNullOrEmpty(afterId))
            {
                queryString = $"{queryString}&after={Uri.EscapeDataString(afterId)}";
            }

            return await base.GetAsync<IEnumerable<Book>>(query: queryString);
        }

        public async Task<Book> Get(string id)
        {
            string escapedId = Uri.EscapeDataString(id);
            return await base.GetAsync<Book>(path: escapedId);
        }

        public async Task<Book> Post(Book book)
        {
            return await base.PostAsync<Book, Book>(book);
        }

        public async Task<Book> Put(string id, Book book)
        {
            string escapedId = Uri.EscapeDataString(id);
            return await base.PutAsync(book, path: escapedId);
        }

        public async Task<IEnumerable<Book>> Query(BookQuery query)
        {
            return await base.PostAsync<BookQuery, IEnumerable<Book>>(query, "Query");
        }
    }
}
