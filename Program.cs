using RetailConnect.API.Services.Interfaces;
using RetailConnect.API.Services.Implementations;
using RetailConnect.API.Data;
using RetailConnect.API.Middleware;
using RetailConnect.API.HealthChecks;
using Serilog;
using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;

try
{
    Log.Information("Starting RetailConnect API");

    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog for structured logging
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console()
        .WriteTo.File("logs/retailconnect-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "RetailConnect")
        .CreateLogger();

    builder.Host.UseSerilog();

    // Validate required configuration
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "Database connection string 'DefaultConnection' is not configured. " +
            "Please ensure appsettings.json or appsettings.Development.json contains a valid connection string.");
    }

    Log.Information("Configuration validated successfully");

// Add services to the container
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = false; // Enable automatic model validation
    });

// Response Compression (Gzip + Brotli)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

// Output Caching for improved performance
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromMinutes(5)));
    options.AddPolicy("GetAll", builder => builder.Expire(TimeSpan.FromMinutes(5)));
    options.AddPolicy("GetById", builder => builder.Expire(TimeSpan.FromMinutes(10)));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "RetailConnect API",
        Version = "v1",
        Description = "Industrial-grade marketing campaign management API with comprehensive CRUD operations for campaigns, segments, templates, and touchpoints.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "RetailConnect Team"
        }
    });

    // Include XML comments for better Swagger documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Register Data Access Layer
builder.Services.AddScoped<SegmentDataAccess>();
builder.Services.AddScoped<CampaignDataAccess>();
builder.Services.AddScoped<TemplateDataAccess>();
builder.Services.AddScoped<TouchpointDataAccess>();
builder.Services.AddScoped<CampaignTouchpointDataAccess>();
builder.Services.AddScoped<CampaignTemplateDataAccess>();
builder.Services.AddScoped<CampaignLogDataAccess>();
builder.Services.AddScoped<DeleteDataAccess>();

// Register Services (Business Logic Layer)
builder.Services.AddScoped<ISegmentService, SegmentService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<ITouchpointService, TouchpointService>();
builder.Services.AddScoped<IDeleteService, DeleteService>();

// Add CORS (must be before builder.Build())
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUI",
        policy => policy
            .WithOrigins("https://localhost:5001", "https://localhost:7229", "http://localhost:5191")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready", "live" });

var app = builder.Build();

    Log.Information("Application built successfully");

    // Configure the HTTP request pipeline

    // Performance monitoring (tracks response times)
    app.UseMiddleware<PerformanceMonitoringMiddleware>();

    // Global exception handler (must be first after performance monitoring)
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    // Response compression
    app.UseResponseCompression();

    // Output caching
    app.UseOutputCache();

    // Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
        };
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "RetailConnect API v1");
            options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        });
    }

    app.UseCors("AllowUI");

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    // Health check endpoints
    app.MapHealthChecks("/health");
    app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });
    app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live")
    });

    Log.Information("Starting web host on {Environment}", app.Environment.EnvironmentName);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

