using document_sharing_manager.Core.Interfaces;
using document_sharing_manager.Infrastructure.Persistence;
using document_sharing_manager.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configure Entity Framework Core with PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");
}

if (connectionString.Contains("${POSTGRES_PASSWORD}"))
{
    var postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
    if (string.IsNullOrEmpty(postgresPassword))
    {
        throw new InvalidOperationException("Database connection string is not configured properly. Ensure POSTGRES_PASSWORD environment variable is set.");
    }
    connectionString = connectionString.Replace("${POSTGRES_PASSWORD}", postgresPassword);
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Repositories
builder.Services.AddScoped<IDocumentRepository, EfDocumentRepository>();
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
