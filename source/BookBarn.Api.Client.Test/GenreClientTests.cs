using BookBarn.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BookBarn.Api.Client.Test
{
    public class GenreClientTests
    {
        [Fact]
        public async Task GetListHasValidRequestResponse()
        {
            Genre sampleGenre = new Genre() { Count = 10, Id = "Fiction" };

            TestMessageHandler handler = new TestMessageHandler();

            handler.ResponseAction = new Func<HttpRequestMessage, HttpResponseMessage>((req) =>
            {
                Assert.Null(req.Content);

                HttpResponseMessage res = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                res.Content = JsonContent.Create<IEnumerable<Genre>>(new List<Genre>() { sampleGenre });

                Uri expected = new Uri("https://example.com/v1/Genres");
                Assert.Equal(expected, req.RequestUri);
                Assert.Equal(HttpMethod.Get, req.Method);

                return res;
            });

            GenreClient client = new GenreClient(new HttpClient(handler), new Uri("https://example.com"));

            var result = await client.Get();
            Assert.Single(result);
            Assert.Equal(sampleGenre.Id, result.Single().Id);
        }

        [Fact]
        public async Task GetSingleHasValidRequestResponse()
        {
            Genre sampleGenre = new Genre() { Count = 10, Id = "Fiction" };

            TestMessageHandler handler = new TestMessageHandler();

            handler.ResponseAction = new Func<HttpRequestMessage, HttpResponseMessage>((req) =>
            {
                Assert.Null(req.Content);

                HttpResponseMessage res = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                res.Content = JsonContent.Create<Genre>(sampleGenre);

                Uri expected = new Uri($"https://example.com/v1/Genres/{sampleGenre.Id}");
                Assert.Equal(expected, req.RequestUri);
                Assert.Equal(HttpMethod.Get, req.Method);

                return res;
            });

            GenreClient client = new GenreClient(new HttpClient(handler), new Uri("https://example.com"));

            var result = await client.Get(sampleGenre.Id);
            Assert.NotNull(result);
            Assert.Equal(sampleGenre.Id, result.Id);
            Assert.Equal(sampleGenre.Count, result.Count);
        }
    }
}
