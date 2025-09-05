using BookBarn.Model;
using BookBarn.Model.Api.v1;

namespace BookBarn.Api.Client
{
    public class BookClient : ApiClient, IBooksService
    {
        public BookClient(Uri endpoint) : base(endpoint) { }

        public BookClient(HttpClient client) : base(client) { }

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

        protected override string GetRoute()
        {
            return "/api/v1/Books";
        }
    }
}
