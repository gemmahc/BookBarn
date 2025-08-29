using BookBarn.Model;
using System.Net.Http.Json;

namespace BookBarn.Api.Client.Test
{
    public class BookClientTests
    {
        [Fact]
        public async Task GetListHasValidRequestResponse()
        {
            TestMessageHandler handler = new TestMessageHandler();

            handler.ResponseAction = new Func<HttpRequestMessage, HttpResponseMessage>((req) =>
            {
                Assert.Null(req.Content);

                HttpResponseMessage res = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                res.Content = JsonContent.Create<IEnumerable<Book>>(new List<Book>() { new Book(), new Book() });

                Uri expected = new Uri("https://example.com/v1/Books?count=25");
                Assert.Equal(expected, req.RequestUri);
                Assert.Equal(HttpMethod.Get, req.Method);

                return res;
            });

            BookClient client = new BookClient(new HttpClient(handler), new Uri("https://example.com"));

            var result = await client.Get(count: 25);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetSingleHasValidRequestResponse()
        {
            string sampleId = "8567a18b529466be726e38a527051a7f";
            TestMessageHandler handler = new TestMessageHandler();

            handler.ResponseAction = new Func<HttpRequestMessage, HttpResponseMessage>((req) =>
            {
                Assert.Null(req.Content);

                HttpResponseMessage res = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                res.Content = JsonContent.Create<Book>(new Book() { Id = sampleId });

                Uri expected = new Uri($"https://example.com/v1/Books/{sampleId}");
                Assert.Equal(expected, req.RequestUri);
                Assert.Equal(HttpMethod.Get, req.Method);

                return res;
            });

            BookClient client = new BookClient(new HttpClient(handler), new Uri("https://example.com"));

            var result = await client.Get(sampleId);
            Assert.NotNull(result);
            Assert.Equal(sampleId, result.Id);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1031:Do not use blocking task operations in test method", Justification = "")]
        public async Task PostBookHasValidRequestResponse()
        {
            string sampleId = "8567a18b529466be726e38a527051a7f";
            Book sampleBook = new Book() { Id = sampleId };
            TestMessageHandler handler = new TestMessageHandler();

            handler.ResponseAction = new Func<HttpRequestMessage, HttpResponseMessage>((req) =>
            {
                Assert.NotNull(req.Content);
                var content = req.Content.ReadFromJsonAsync<Book>().GetAwaiter().GetResult();
                Assert.NotNull(content);
                Assert.Equal(sampleBook.Id, content.Id);

                HttpResponseMessage res = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                res.Content = JsonContent.Create<Book>(content);

                Uri expected = new Uri($"https://example.com/v1/Books");
                Assert.Equal(expected, req.RequestUri);
                Assert.Equal(HttpMethod.Post, req.Method);

                return res;
            });

            BookClient client = new BookClient(new HttpClient(handler), new Uri("https://example.com"));

            var result = await client.Post(sampleBook);
            Assert.NotNull(result);
            Assert.Equal(sampleBook.Id, result.Id);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1031:Do not use blocking task operations in test method", Justification = "")]
        public async Task PutBookHasValidRequestResponse()
        {
            string sampleId = "8567a18b529466be726e38a527051a7f";
            Book sampleBook = new Book() { Id = sampleId };
            TestMessageHandler handler = new TestMessageHandler();

            handler.ResponseAction = new Func<HttpRequestMessage, HttpResponseMessage>((req) =>
            {
                Assert.NotNull(req.Content);

                var content = req.Content.ReadFromJsonAsync<Book>().GetAwaiter().GetResult();
                Assert.NotNull(content);
                Assert.Equal(sampleBook.Id, content.Id);

                HttpResponseMessage res = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                res.Content = JsonContent.Create<Book>(content);

                Uri expected = new Uri($"https://example.com/v1/Books/{sampleId}");
                Assert.Equal(expected, req.RequestUri);
                Assert.Equal(HttpMethod.Put, req.Method);

                return res;
            });

            BookClient client = new BookClient(new HttpClient(handler), new Uri("https://example.com"));

            var result = await client.Put(sampleId, sampleBook);
            Assert.NotNull(result);
            Assert.Equal(sampleBook.Id, result.Id);
        }

        [Fact]
        public async Task DeleteHasValidRequestResponse()
        {
            string sampleId = "8567a18b529466be726e38a527051a7f";
            TestMessageHandler handler = new TestMessageHandler();

            handler.ResponseAction = new Func<HttpRequestMessage, HttpResponseMessage>((req) =>
            {
                Assert.Null(req.Content);

                HttpResponseMessage res = new HttpResponseMessage(System.Net.HttpStatusCode.OK);

                Uri expected = new Uri($"https://example.com/v1/Books/{sampleId}");
                Assert.Equal(expected, req.RequestUri);
                Assert.Equal(HttpMethod.Delete, req.Method);

                return res;
            });

            BookClient client = new BookClient(new HttpClient(handler), new Uri("https://example.com"));

            await client.Delete(sampleId);

        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1031:Do not use blocking task operations in test method", Justification = "")]
        public async Task QueryHasValidRequestResponse()
        {
            string sampleId = "8567a18b529466be726e38a527051a7f";
            BookQuery sampleQuery = new BookQuery() { Author = "Steve Irwin" };

            Book sampleBook = new Book() { Id = sampleId };
            TestMessageHandler handler = new TestMessageHandler();

            handler.ResponseAction = new Func<HttpRequestMessage, HttpResponseMessage>((req) =>
            {
                Assert.NotNull(req.Content);

                var content = req.Content.ReadFromJsonAsync<BookQuery>().GetAwaiter().GetResult();
                Assert.NotNull(content);
                Assert.Equal(sampleQuery.Author, content.Author);

                HttpResponseMessage res = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                res.Content = JsonContent.Create<IEnumerable<Book>>(new List<Book>() { sampleBook });

                Uri expected = new Uri($"https://example.com/v1/Books/Query");
                Assert.Equal(expected, req.RequestUri);
                Assert.Equal(HttpMethod.Post, req.Method);

                return res;
            });

            BookClient client = new BookClient(new HttpClient(handler), new Uri("https://example.com"));

            var result = await client.Query(sampleQuery);
            Assert.Single(result);
            Assert.Equal(sampleBook.Id, result.Single().Id);
        }
    }
}