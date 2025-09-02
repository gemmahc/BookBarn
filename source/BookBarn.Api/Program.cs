using BookBarn.Api.Providers;
using BookBarn.Api.v1;
using BookBarn.Model.Providers;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add data providers to container
string? dataSourceConn = builder.Configuration.GetConnectionString("DataSource");
string? database = builder.Configuration["DataSource:Database"];
string? bookCollection = builder.Configuration["DataSource:BookCollection"];
string? genreCollection = builder.Configuration["DataSource:GenreCollection"];
string? mediaStorage = builder.Configuration.GetConnectionString("MediaSource");
string? mediaSourceContainer = builder.Configuration["MediaSource:Container"];

if (string.IsNullOrEmpty(database) || string.IsNullOrEmpty(bookCollection) || string.IsNullOrEmpty(genreCollection) || string.IsNullOrEmpty(dataSourceConn))
{
    throw new InvalidOperationException("Unable to start without valid datasource configuration.");
}

builder.Services.AddSingleton<IBookDataProvider>(dp => new BookDataProvider(dataSourceConn, database, bookCollection));
builder.Services.AddSingleton<IGenreDataProvider>(dp => new GenreDataProvider(dataSourceConn, database, genreCollection));

if (string.IsNullOrEmpty(mediaStorage) || string.IsNullOrEmpty(mediaSourceContainer))
{
    throw new InvalidOperationException("Unable to start without valid media storage configuration.");
}

builder.Services.AddSingleton<IMediaStorageProvider>(ms => new MediaStorageProvider(mediaStorage, mediaSourceContainer));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
