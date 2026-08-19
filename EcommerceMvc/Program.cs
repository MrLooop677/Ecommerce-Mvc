using EcommerceMvc;
using EcommerceMvc.Configurations;
using EcommerceMvc.Utitlies.DBInitializer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = null);


// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString =
        builder.Configuration.GetConnectionString("DefaultConnection")
      ?? throw new InvalidOperationException("Connection string"
      + "'DefaultConnection' not found.");

//AppConfiguration.RegisterConfiguration(builder.Services, connectionString);
builder.Services.RegisterConfiguration( connectionString, builder.Configuration);
builder.Services.RegisterMapsterConfig(); 
var app = builder.Build();
var scope=app.Services.CreateScope();
var services = scope.ServiceProvider.GetService<IDBInitializer>();
services?.Initialize();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=customer}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
