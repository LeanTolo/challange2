using Challange.Config;
using Challange.Data;
using Challange.RabitMQ;
using Challange.RabitMQ.Interfaces;
using Challange.Services;
using Challange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
var RabbitConfig = builder.Configuration
    .GetSection("RabbitConfigDireccion")
    .Get<RabbitConfig>();

// Add services to the container.
builder.Services.AddScoped<IGeoService, GeoService>();
builder.Services.AddSingleton(RabbitConfig);
builder.Services.AddScoped<IRabitMQDireccion, RabitMqDireccion>();

//builder.Services.AddHostedService<BackgroundRabbitService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//scope para levantar las migraciones en la base local dockerizada
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();