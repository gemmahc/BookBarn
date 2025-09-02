using BookBarn.Model;
using BookBarn.Model.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookBarn.Crawler.GoodReads.Test
{
    public class TestMediaProvider : IMediaStorageProvider
    {
        public Task<MediaStorageToken> CreateWriteToken(string id, TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Media?> GetMetadata(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Media> UpsertFrom(Uri source)
        {
            throw new NotImplementedException();
        }

        public Task<Media> UpsertFrom(string id, Uri source)
        {
            throw new NotImplementedException();
        }
    }
}
