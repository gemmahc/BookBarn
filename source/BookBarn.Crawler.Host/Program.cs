using BookBarn.Api.Client;
using BookBarn.Api.v1;
using BookBarn.Crawler;
using BookBarn.Crawler.GoodReads;
using BookBarn.Crawler.Host;

var builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
}

var hostConfigSection = builder.Configuration.GetSection(nameof(CrawlerHostConfiguration));
builder.Services.Configure<CrawlerHostConfiguration>(hostConfigSection);

builder.Services.AddHostedService<Worker>();

string? bookBarnApi = builder.Configuration["BookBarnApi:ServiceEndpoint"];

if(bookBarnApi == null)
{
    throw new InvalidOperationException("Unable to run without BookBarnApi endpoint configured.");
}

builder.Services.AddHttpClient(); // default factory

// Typed client for book api
builder.Services.AddHttpClient<IBooksService, BookClient>(client =>
{
    client.BaseAddress = new Uri(bookBarnApi);
    client.DefaultRequestHeaders.Add("X-Correlation-ID", Guid.NewGuid().ToString());
});

// Typed client for media api
builder.Services.AddHttpClient<IMediaService, MediaClient>(client =>
{
    client.BaseAddress = new Uri(bookBarnApi);
    client.DefaultRequestHeaders.Add("X-Correlation-ID", Guid.NewGuid().ToString());
});

builder.Services.AddSingleton<ICrawlerFactory, GoodReadsCrawlerFactory>();
builder.Services.AddSingleton<ICrawlerQueue, InMemoryQueue>();

var host = builder.Build();
host.Run();
