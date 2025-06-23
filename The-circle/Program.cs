using The_circle.Application;
using The_circle.Application.Services;
using The_circle.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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