using Microsoft.OpenApi.Models;
using WebApp.Data;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

// DB Context
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ShirtStoreManagement"));
});

// Controllers
builder.Services.AddControllers();

// Minimal API OpenAPI Support (.NET 10)
builder.Services.AddOpenApi();

// If you need to customize OpenAPI Document
builder.Services.Configure<OpenApiDocument>(document =>
{
    document.Components ??= new OpenApiComponents();

    document.Components.SecuritySchemes.TryAdd("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    document.SecurityRequirements.Add(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddApiVersioning(options =>
{
    // Default API Version
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new QueryStringApiVersionReader("api-version")  
    );
})
.AddApiExplorer();

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if(app.Environment.IsDevelopment())
{
    Console.WriteLine("Discovered API versions: ");
    foreach(var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        Console.WriteLine($"- {description.GroupName} (V {description.ApiVersion})");
    }
}

// Enable OpenAPI JSON (from .NET 10 Minimal API pipeline)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Exposes /openapi/v1.json

    // Swagger UI will use that endpoint:
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "WebAPIApp API V1");
        options.SwaggerEndpoint("/openapi/v2.json", "WebAPIApp API V1");
        options.RoutePrefix = string.Empty; // Optional: serve at root "/"
    });
}

//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();
