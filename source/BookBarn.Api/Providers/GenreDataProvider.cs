using BookBarn.Model;
using BookBarn.Model.Providers;

namespace BookBarn.Api.Providers
{
    public class GenreDataProvider : IGenreDataProvider
    {
        public GenreDataProvider(string connectionString, string database, string collection)
        {

        }

        public Task<IEnumerable<Genre>> GetGenres()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Genre>> QueryGenres(GenreQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
