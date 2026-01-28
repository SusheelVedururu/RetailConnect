using RetailConnect.API.Services.Interfaces;
using RetailConnect.API.Services.Implementations;
using RetailConnect.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Data Access Layer
builder.Services.AddScoped<SegmentDataAccess>();
builder.Services.AddScoped<CampaignDataAccess>();
builder.Services.AddScoped<TemplateDataAccess>();
builder.Services.AddScoped<TouchpointDataAccess>();
builder.Services.AddScoped<CampaignTouchpointDataAccess>();
builder.Services.AddScoped<CampaignTemplateDataAccess>();
builder.Services.AddScoped<CampaignLogDataAccess>();

// Register Services (Business Logic Layer)
builder.Services.AddScoped<ISegmentService, SegmentService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<ITouchpointService, TouchpointService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

