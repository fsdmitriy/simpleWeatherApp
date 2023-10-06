using Microsoft.EntityFrameworkCore;
using SimpleWeatherApp.Data;
using SimpleWeatherApp.Repositories;
using SimpleWeatherApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<WeatherDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<IWeatherDataRepository, WeatherDataRepository>();

// Register the task scheduler
builder.Services.AddSingleton<IHostedService, WeatherDataRefreshService>();

var app = builder.Build();

// Migrate latest database changes during startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<WeatherDbContext>();

    dbContext?.Database.EnsureCreated();
    dbContext?.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
