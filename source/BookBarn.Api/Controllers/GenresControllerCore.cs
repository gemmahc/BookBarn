using BookBarn.Api.ErrorHandling;
using BookBarn.Model;
using BookBarn.Model.Api.v1;
using BookBarn.Model.Providers;

namespace BookBarn.Api.Controllers
{
    public class GenresControllerCore : IGenresService
    {
        private IGenreDataProvider _genreProvider;
        private ILogger _logger;

        public GenresControllerCore(IGenreDataProvider genreProvider, ILogger logger)
        {
            _genreProvider = genreProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<Genre>> Get()
        {
            var genres = await _genreProvider.GetGenres();

            if (genres == null)
            {
                throw new DataException(DataError.General);
            }

            return genres;
        }

        public async Task<Genre> Get(string name)
        {
            var query = new GenreQuery()
            {
                Id = name
            };

            var genres = await _genreProvider.QueryGenres(query);

            if (genres == null)
            {
                throw new DataException(DataError.General);
            }
            if (genres.Count() != 1)
            {
                throw new DataException(DataError.NotFound);
            }

            return genres.Single();
        }
    }
}
