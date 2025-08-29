using System.Net.Http.Json;

namespace BookBarn.Api.Client
{
    /// <summary>
    /// Common Api Client for basic http operations.
    /// </summary>
    public abstract class ApiClient
    {
        private Uri _endpoint;
        private static HttpClient _client = new HttpClient();

        protected ApiClient(Uri endpoint)
        {
            _endpoint = endpoint;
        }

        protected ApiClient(HttpClient client, Uri endpoint) : this(endpoint)
        {
            _client = client;
        }

        protected async Task<T> GetAsync<T>(string? path = null, string? query = null)
        {
            Uri target = BuildTarget(path: path, query: query);

            var res = await _client.GetAsync(target);

            return await ProcessResult<T>(res);
        }

        protected async Task<T> PutAsync<T>(T obj, string? path = null)
        {
            Uri target = BuildTarget(path: path);

            var res = await _client.PutAsJsonAsync<T>(target, obj);

            return await ProcessResult<T>(res);
        }

        protected async Task<T2> PostAsync<T1, T2>(T1 obj, string? action = null)
        {
            Uri target = BuildTarget(path: action);

            var res = await _client.PostAsJsonAsync<T1>(target, obj);

            return await ProcessResult<T2>(res);
        }

        protected async Task DeleteAsync(string id)
        {
            Uri target = BuildTarget(path: id);

            var res = await _client.DeleteAsync(target);

            res.EnsureSuccessStatusCode();
        }

        private Uri BuildTarget(string? path = null, string? query = null)
        {
            string pathAndQuery = string.Empty;

            UriBuilder builder = new UriBuilder(_endpoint);

            if (!string.IsNullOrEmpty(path))
            {
                builder.Path = $"{builder.Path}/{path}";
            }

            if (!string.IsNullOrEmpty(query))
            {
                string existingQuery = string.IsNullOrEmpty(builder.Query) ? string.Empty : $"{builder.Query}&";
                builder.Query = $"{existingQuery}{query}";
            }

            return builder.Uri;
        }

        private async Task<T> ProcessResult<T>(HttpResponseMessage result)
        {
            var success = result.EnsureSuccessStatusCode();

            T? content = await success.Content.ReadFromJsonAsync<T>();

            if (content == null)
            {
                throw new Exception("Expected content not returned");
            }

            return content;
        }
    }
}
