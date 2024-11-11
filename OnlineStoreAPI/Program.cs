using OnlineStoreAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ????????? CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        policy => policy.WithOrigins("https://localhost:7151") // ????, ?? ??????? ???????? Blazor-??????????
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// ??????????? ? ???? ??????
builder.Services.AddDbContext<StoreContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("StoreDatabase"),
    new MySqlServerVersion(new Version(8, 0, 25))));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ?????????? CORS-????????
app.UseCors("AllowBlazorApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
