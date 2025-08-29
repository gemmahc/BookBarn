namespace BookBarn.Api.Client.Test
{
    public class TestMessageHandler : HttpMessageHandler
    {
        public TestMessageHandler()
        {
            ResponseAction = new Func<HttpRequestMessage, HttpResponseMessage>((req) =>
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            });
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(ResponseAction.Invoke(request));
        }

        public Func<HttpRequestMessage, HttpResponseMessage> ResponseAction { get; set; }
    }
}
