using BookBarn.Model;
using BookBarn.Model.Providers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace BookBarn.Api.Providers
{
    public class BookDataProvider : IBookDataProvider, IDisposable
    {
        private MongoClient? _client;
        private Lazy<IMongoCollection<Book>> _bookCollection;
        private bool _disposedValue;

        public BookDataProvider(string connectionString, string database, string collection)
        {
            _bookCollection = new Lazy<IMongoCollection<Book>>(() =>
            {
                BsonClassMap.RegisterClassMap<Book>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });

                _client = new MongoClient(connectionString);
                return _client.GetDatabase(database).GetCollection<Book>(collection);
            });
        }

        public async Task<bool> Delete(string id)
        {
            var filter = Builders<Book>.Filter.Eq(b => b.Id, id);
            var result = await _bookCollection.Value.DeleteOneAsync(filter);

            return result.IsAcknowledged;
        }

        public async Task<Book> Get(string id)
        {
            var filter = Builders<Book>.Filter.Eq(b => b.Id, id);
            var result = await _bookCollection.Value.FindAsync<Book>(filter);
            return await result.SingleOrDefaultAsync<Book>();
        }

        public async Task<IEnumerable<Book>> GetMany(int count, string? afterId)
        {
            var filter = Builders<Book>.Filter.Gt(b => b.Id, afterId ?? "");

            return await _bookCollection.Value.Find<Book>(filter).Limit(count).ToListAsync();
        }

        public async Task<IEnumerable<Book>> QueryBooks(BookQuery query)
        {
            var filter = BuildBookFilter(query);

            var res = await _bookCollection.Value.FindAsync<Book>(filter);

            return await res.ToListAsync();
        }

        public async Task<Book> Upsert(Book book)
        {
            book.LastUpdated = DateTime.UtcNow;
            var result = await _bookCollection.Value.ReplaceOneAsync<Book>(b => b.Id == book.Id, book, new ReplaceOptions() { IsUpsert = true });
            return book;
        }

        private FilterDefinition<Book> BuildBookFilter(BookQuery query)
        {
            List<FilterDefinition<Book>> filters = new List<FilterDefinition<Book>>();

            if (!string.IsNullOrEmpty(query.Author))
            {
                filters.Add(Builders<Book>.Filter.Eq(b => b.Author, query.Author));
            }

            if (!string.IsNullOrEmpty(query.Title))
            {
                filters.Add(Builders<Book>.Filter.Eq(b => b.Title, query.Title));
            }

            if (query.RatingFloor.HasValue)
            {
                filters.Add(Builders<Book>.Filter.Gte(b => b.Rating, query.RatingFloor.Value));
            }

            if (query.MinRatings.HasValue)
            {
                filters.Add(Builders<Book>.Filter.Gte(b => b.RatingCount, query.MinRatings.Value));
            }

            if (query.FirstInSeries.HasValue)
            {
                if (query.FirstInSeries.Value)
                {
                    filters.Add(Builders<Book>.Filter.Eq(b => b.SeriesRank, "1"));
                }
                else
                {
                    filters.Add(Builders<Book>.Filter.Ne(b => b.SeriesRank, "1"));
                }
            }

            if (!string.IsNullOrEmpty(query.DescriptionContains))
            {
                filters.Add(Builders<Book>.Filter.Regex(b => b.Description, BsonRegularExpression.Create(query.DescriptionContains)));
            }

            string genresField = "Genres";

            if ((query.MatchAllIncludedGenres.HasValue && query.MatchAllIncludedGenres.Value) || query.IncludedGenres?.Length == 1)
            {
                foreach (string genre in query.IncludedGenres ?? [])
                {
                    filters.Add(Builders<Book>.Filter.Eq(genresField, genre));
                }
            }
            else if (query.IncludedGenres?.Length > 1)
            {
                List<FilterDefinition<Book>> genreOr = new List<FilterDefinition<Book>>();

                foreach (string genre in query.IncludedGenres ?? [])
                {
                    genreOr.Add(Builders<Book>.Filter.Eq(genresField, genre));
                }

                filters.Add(Builders<Book>.Filter.Or(genreOr));
            }

            foreach (string genre in query.ExcludedGenres ?? [])
            {
                filters.Add(Builders<Book>.Filter.Ne(genresField, genre));
            }

            var queryFilter = Builders<Book>.Filter.And(filters);

            return queryFilter;
        }

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_client != null)
                    {
                        _client.Dispose(disposing);
                        _client = null;
                    }
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
        #endregion
    }
}
