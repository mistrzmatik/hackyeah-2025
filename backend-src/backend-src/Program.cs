using backend_src;
using backend_src.Kalkulatorki;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IKalkulatorEmerytury, KalkulatorEmerytury>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var connString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(connString, sql =>
    {
        sql.EnableRetryOnFailure(maxRetryCount: 5,
                                 maxRetryDelay: TimeSpan.FromSeconds(5),
                                 errorNumbersToAdd: null);
    }));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    const int attempts = 10;
    for (int i = 1; i <= attempts; i++)
    {
        try
        {
            await db.Database.MigrateAsync();

            break;
        }
        catch (SqlException)
        {
            if (i == attempts) throw;
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
        catch (Exception)
        {
            if (i == attempts) throw;
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
