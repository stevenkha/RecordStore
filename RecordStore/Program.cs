using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecordStore.Data;
using RecordStore.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<RecordStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RecordStoreContext") ?? throw new InvalidOperationException("Connection string 'RecordStoreContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    DbInitializer.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
