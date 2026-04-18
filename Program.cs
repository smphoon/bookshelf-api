using BookshelfApi.Repositories;
using BookshelfApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Scoped lifetime: one instance per HTTP request.
builder.Services.AddScoped<IBooksRepository, BooksRepository>();
builder.Services.AddScoped<IBooksService, BooksService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpa", policy =>
    {
        policy.WithOrigins(
                "http://127.0.0.1:5500",
                "http://localhost:5500",
                "https://icy-smoke-023496c03.7.azurestaticapps.net")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowSpa");
app.UseAuthorization();
app.MapControllers();

app.Run();