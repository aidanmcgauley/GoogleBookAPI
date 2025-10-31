using GoogleBookAPI.Data;
using GoogleBookAPI.Repositories;
using GoogleBookAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<BookIngestionService>(); // registers IHttpClientFactory
builder.Services.AddScoped<BookIngestionService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddDbContext<BookDbContext>(options
    => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Run ingestion at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var ingestionService = services.GetRequiredService<BookIngestionService>();
    var configuration = services.GetRequiredService<IConfiguration>();
    var jsonFileUrl = configuration["JsonFileUrl"]; // add this to your appsettings.json

    await ingestionService.IngestDataFromEndpoint(jsonFileUrl);
}

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
