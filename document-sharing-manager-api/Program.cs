using System.Text;
using document_sharing_manager.Core.Interfaces;
using document_sharing_manager.Core.Configurations;
using document_sharing_manager.Infrastructure.Persistence;
using document_sharing_manager.Infrastructure.Persistence.Repositories;
using document_sharing_manager.Infrastructure.Security;
using document_sharing_manager.Infrastructure.Storage;
using document_sharing_manager_api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using document_sharing_manager_api.Filters;

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

// Register Auth Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register Storage Services
builder.Services.AddScoped<IStorageService, LocalFileStorageService>();

// Resolve JWT Secret from Environment if placeholder is used
var jwtSection = builder.Configuration.GetSection("JWT");
var jwtKey = jwtSection["Key"];
if (jwtKey != null && jwtKey.Contains("${JWT_SECRET_KEY}"))
{
    jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
    builder.Configuration["JWT:Key"] = jwtKey;
}

if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException("JWT Key must be at least 32 characters long and configured properly via environment variable (JWT_SECRET_KEY) or appsettings.");
}

// Bind Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection("Security"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Configure Global Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Document Sharing Manager API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.OperationFilter<AuthorizeCheckOperationFilter>();

    // Include XML Comments for Examples
    var apiXmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
    if (File.Exists(apiXmlPath)) c.IncludeXmlComments(apiXmlPath);

    var coreXmlPath = Path.Combine(AppContext.BaseDirectory, "document-sharing-manager.Core.xml");
    if (File.Exists(coreXmlPath)) c.IncludeXmlComments(coreXmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler(); // Must be first

app.UseSwagger(options =>
{
    options.RouteTemplate = "openapi/{documentName}.json";
});
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger";
});

app.MapScalarApiReference(options => 
{
    options.WithTitle("DocShare Management API")
           .WithTheme(ScalarTheme.Kepler)
           .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl)
           .WithDocumentDownloadType(DocumentDownloadType.None);
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Automatic Migration for Docker/Environment
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // Check if database can be connected to, with retries (Docker DB might take a while to start)
        int retryCount = 0;
        bool connected = false;
        while (retryCount < 10 && !connected)
        {
            try
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
                connected = true;
            }
            catch
            {
                retryCount++;
                Console.WriteLine($"Waiting for Database to be ready... (Attempt {retryCount}/10)");
                Thread.Sleep(3000);
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();
