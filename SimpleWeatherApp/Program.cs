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

#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
var dbContext = builder.Services?.BuildServiceProvider().GetService<WeatherDbContext>();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
if (dbContext is not null)
{
    dbContext.Database.Migrate();
}

var app = builder.Build();

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
