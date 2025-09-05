using BookBarn.Model;
using BookBarn.Model.Api.v1;

namespace BookBarn.Api.Client
{
    public class GenreClient : ApiClient, IGenresService
    {
        public GenreClient(Uri endpoint) : base(endpoint) { }

        public GenreClient(HttpClient client) : base(client) { }

        public async Task<IEnumerable<Genre>> Get()
        {
            return await base.GetAsync<IEnumerable<Genre>>();
        }

        public async Task<Genre> Get(string name)
        {
            return await base.GetAsync<Genre>(path: name);
        }

        protected override string GetRoute()
        {
            return "/api/v1/Genres";
        }
    }
}
