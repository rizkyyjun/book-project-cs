using BookProject;
using BookProject.Interface;
using BookProject.Model;
using BookProject.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add DbContext for SQL Server
builder.Services.AddDbContext<BookProjectDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


// Add AutoMapper to the DI container
//builder.Services.AddAutoMapper(typeof(AutoMapperProfileConfiguration).Assembly);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sample Book Project",
        Version = "v1"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample Book Project");
    });
}   

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
