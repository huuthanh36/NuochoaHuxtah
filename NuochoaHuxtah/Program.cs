using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuochoaHuxtah.Repository;

var builder = WebApplication.CreateBuilder(args);
//Connncetion DB
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

app.UseSession();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");
//Route Category
app.MapControllerRoute(
    name: "category",
    pattern: "/category/{slug?}",
    defaults: new {Controller="Category", action="Index"});
//Route Brand
app.MapControllerRoute(
    name: "brand",
    pattern: "/brand/{slug?}",
    defaults: new { Controller = "Brand", action = "Index" });



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Seeding data
var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedingData(context);

app.Run();
