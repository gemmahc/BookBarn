using BookBarn.Api;
using BookBarn.Api.Providers;
using BookBarn.Model.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Configuration config = new Configuration(builder.Configuration);

// Add logging.
if (builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
}
else
{
    // production logger impl (seilog/appinsight, don't know yet)
}

if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{config.KeyVaultName}.vault.azure.net/"),
        config.ServiceCredential,
        new KeyVaultSecretProvider(builder.Environment.EnvironmentName.ToLowerInvariant()));
}

// Add data providers to container
builder.Services.AddSingleton<IBookDataProvider>(dp => new BookDataProvider(config.DataSourceConnectionString, config.DataSourceDatabase, config.DataSourceBookCollection));
builder.Services.AddSingleton<IGenreDataProvider>(dp => new GenreDataProvider(config.DataSourceConnectionString, config.DataSourceDatabase, config.DataSourceGenreCollection));

if (builder.Environment.IsDevelopment())
{
    // Use emulator storage - no auth.
    builder.Services.AddSingleton<IMediaStorageProvider>(ms => new MediaStorageProvider(config.MediaStoreConnectionString, config.MediaStoreContainer));
}
else
{
    // Use storage account with service credential.
    builder.Services.AddSingleton<IMediaStorageProvider>(ms => new MediaStorageProvider(new Uri(config.MediaStoreConnectionString), config.ServiceCredential));
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

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
