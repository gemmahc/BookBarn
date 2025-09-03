using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.ObjectPool;

namespace BookBarn.Api
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private const string CORRELATION_HEADER = "X-Correlation-ID";

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? correlationId = string.Empty;
            if (context.Request.Headers.ContainsKey(CORRELATION_HEADER))
            {
                var correlation = context.Request.Headers[CORRELATION_HEADER];
                correlationId = correlation.FirstOrDefault();
            }

            if (string.IsNullOrEmpty(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId
            }))
            {
                _logger.LogInformation("Request [{method}][{path}] with correlationId [{CorrelationId}]", context.Request.Method, context.Request.Path, correlationId);
                context.Response.OnStarting(state =>
                {
                    var httpContext = (HttpContext)state;
                    if (!httpContext.Response.Headers.ContainsKey(CORRELATION_HEADER))
                    {
                        httpContext.Response.Headers.Append(CORRELATION_HEADER, correlationId);
                    }

                    return Task.CompletedTask;
                }, context);

                await _next(context);
            }
        }
    }
}
