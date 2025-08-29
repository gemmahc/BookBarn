using BookBarn.Model;
using BookBarn.Model.API.v1;

namespace BookBarn.Api.Client
{
    public class GenreClient : ApiClient, IGenresController
    {
        public GenreClient(Uri endpoint) : base(new Uri(endpoint, "/v1/Genres")) { }

        public GenreClient(HttpClient client, Uri endpoint) : base(client, new Uri(endpoint, "/v1/Genres")) { }

        public async Task<IEnumerable<Genre>> Get()
        {
            return await base.GetAsync<IEnumerable<Genre>>();
        }

        public async Task<Genre> Get(string name)
        {
            return await base.GetAsync<Genre>(path: name);
        }
    }
}
