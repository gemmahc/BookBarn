using BookBarn.Model;
using BookBarn.Model.Providers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace BookBarn.Api.Providers
{
    public class GenreDataProvider : IGenreDataProvider, IDisposable
    {
        private MongoClient? _client;
        private Lazy<IMongoCollection<Genre>> _genreCollection;
        private bool _disposedValue;

        public GenreDataProvider(string connectionString, string database, string collection)
        {
            _genreCollection = new Lazy<IMongoCollection<Genre>>(() =>
            {
                BsonClassMap.RegisterClassMap<Genre>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });

                _client = new MongoClient(connectionString);
                return _client.GetDatabase(database).GetCollection<Genre>(collection);
            });
        }

        public async Task<IEnumerable<Genre>> GetGenres()
        {
            var filter = Builders<Genre>.Filter.Empty;

            return await _genreCollection.Value.Find<Genre>(filter).ToListAsync();
        }

        public async Task<IEnumerable<Genre>> QueryGenres(GenreQuery query)
        {
            var filter = Builders<Genre>.Filter.Eq(g => g.Id, query.Id);

            return await _genreCollection.Value.Find<Genre>(filter).ToListAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _client?.Dispose(disposing);
                    _client = null;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
