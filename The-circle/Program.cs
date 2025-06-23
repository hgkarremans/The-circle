using System;
using Microsoft.EntityFrameworkCore;
using The_circle.Application;
using The_circle.Application.Services;
using The_circle.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);   
    options.ListenAnyIP(5001, listenOptions => 
        listenOptions.UseHttps()); 
});

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly    = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout        = TimeSpan.FromMinutes(30);
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=thecircle.db"));

builder.Services.AddSingleton<IUserCameraWriteRepository, InMemoryUserCameraWriteRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddSingleton<VideoFrameBufferService>();
builder.Services.AddHostedService<UdpVideoListenerService>();
builder.Services.AddScoped<IUserCameraWriteRepository, SqlLiteUserCameraWriteRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();