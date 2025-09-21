using System.Text.Json.Serialization;
using LibraryManagement.Infrastructure.Persistence.Scaffolded; 
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<LibraryDbContextScaffold>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));


var allowedOrigins = new[]
{
    "http://localhost:4200",
    "http://localhost:4300",   
    "https://localhost:4200"  
};

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("ui", p => p
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
    
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1");
    c.RoutePrefix = string.Empty; 
});

app.UseCors("ui");


// app.UseHttpsRedirection();

app.MapControllers();
app.Run();
