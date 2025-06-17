using The_circle.Application;
using The_circle.Infrastructure;
using The_circle.Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IUserCameraWriteRepository, InMemoryUserCameraWriteRepository>();
builder.Services.AddSignalR();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();           // <-- THIS must come before MapHub

app.UseAuthorization();

app.MapHub<CameraHub>("/cameraHub");  // Map endpoints after UseRouting

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();