using OnlineStoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using OnlineStoreAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Настройка CORS для взаимодействия с Blazor-прилением
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        policy => policy.WithOrigins("https://localhost:7151") // URL Blazor-приложения
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Настройка контекста базы данных с использованием MySQL
builder.Services.AddDbContext<StoreContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("StoreDatabase"),
    new MySqlServerVersion(new Version(8, 0, 25))));

// Настройка сериализации JSON с поддержкой циклических зависимостей
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddScoped<EmailService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();




// Подключение CORS
app.UseCors("AllowBlazorApp");

if (app.Environment.IsDevelopment())
{
    // Включение Swagger только в режиме разработки
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Маршрутизация контроллеров
app.MapControllers();

app.Run();
