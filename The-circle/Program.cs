using System;
using The_circle.Application;
using The_circle.Application.Services;
using The_circle.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ── Bind Kestrel to any IP on ports 5000 (HTTP) and 5001 (HTTPS)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);   // HTTP
    options.ListenAnyIP(5001, listenOptions => 
        listenOptions.UseHttps()); // HTTPS
});

// ── MVC + Sessions + DI
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout        = TimeSpan.FromMinutes(30);
});

builder.Services.AddSingleton<IUserCameraWriteRepository, InMemoryUserCameraWriteRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddSingleton<VideoFrameBufferService>();
builder.Services.AddHostedService<UdpVideoListenerService>();

var app = builder.Build();

// ── Error pages & security
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ── Session & Auth
app.UseSession();
app.UseAuthorization();

// ── MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();