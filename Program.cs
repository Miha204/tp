using Microsoft.EntityFrameworkCore;
using TP_Conditer.Data;
using TP_Conditer.Services;
using TP_Conditer.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// DATABASE
// ==============================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ==============================
// SERVICES
// ==============================
builder.Services.AddScoped<IClientService, ClientService>();

// ==============================
// CONTROLLERS
// ==============================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// ==============================
// SWAGGER
// ==============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ==============================
// CORS FOR REACT
// ==============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        });
});

var app = builder.Build();

// ==============================
// AUTO MIGRATE — создаёт таблицы при первом запуске
// ==============================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// ==============================
// SWAGGER
// ==============================
app.UseSwagger();
app.UseSwaggerUI();

// ==============================
// CORS — ПЕРВЫМ, до всего остального
// ==============================
app.UseCors("AllowReact");

// ==============================
// HTTPS
// ==============================
app.UseHttpsRedirection();

// ==============================
// AUTH
// ==============================
app.UseAuthorization();

// ==============================
// MAP CONTROLLERS
// ==============================
app.MapControllers();

// ==============================
// RUN
// ==============================
app.Run();