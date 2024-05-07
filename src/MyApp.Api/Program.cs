using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();

// Add Clean Architecture Layer Services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApp", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local") 
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpRequest) =>
    {
        if (httpRequest.Headers.ContainsKey("X-Base-Url") && httpRequest.Headers.ContainsKey("X-Host"))
        {
            var basePath = httpRequest.Headers["X-Base-Url"].ToString();
            var host = httpRequest.Headers["X-Host"].ToString();
            var serverUrl = $"https://{host}{basePath}";
            swaggerDoc.Servers = new List<OpenApiServer> { new() { Url = serverUrl } };
        }
    });
});
app.UseSwaggerUI();

// Endpoints
app.MapHealthChecks("/").AllowAnonymous();

app.Run();