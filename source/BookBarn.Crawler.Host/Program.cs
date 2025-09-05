using BookBarn.Api.Client;
using BookBarn.Crawler;
using BookBarn.Crawler.GoodReads;
using BookBarn.Crawler.Host;
using BookBarn.Crawler.Utilities;
using BookBarn.Model.Api.v1;

//var builder = Host.CreateApplicationBuilder(args);

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
}

var hostConfigSection = builder.Configuration.GetSection(nameof(CrawlerHostConfiguration));
builder.Services.Configure<CrawlerHostConfiguration>(hostConfigSection);

var throttleConfigSection = builder.Configuration.GetSection(nameof(RequestThrottleOptions));
builder.Services.Configure<RequestThrottleOptions>(throttleConfigSection);

builder.Services.AddHostedService<Worker>();

string? bookBarnApi = builder.Configuration["BookBarnApi:ServiceEndpoint"];

if (bookBarnApi == null)
{
    throw new InvalidOperationException("Unable to run without BookBarnApi endpoint configured.");
}

builder.Services.AddHttpClient(); // default factory

// Typed client for book api
builder.Services.AddHttpClient<IBooksService, BookClient>(client =>
{
    client.BaseAddress = new Uri(bookBarnApi);
});

// Typed client for media api
builder.Services.AddHttpClient<IMediaService, MediaClient>(client =>
{
    client.BaseAddress = new Uri(bookBarnApi);
});

builder.Services.AddSingleton<IRequestThrottle, PartitionedRequestThrottle>();
builder.Services.AddSingleton<ICrawlerFactory, GoodReadsCrawlerFactory>();
builder.Services.AddSingleton<ICrawlerQueue, InMemoryQueue>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var host = builder.Build();

// Configure the HTTP request pipeline.
if (host.Environment.IsDevelopment())
{
    host.UseSwagger();
    host.UseSwaggerUI();
}

host.UseHttpsRedirection();

host.UseAuthorization();

host.MapControllers();

host.Run();
