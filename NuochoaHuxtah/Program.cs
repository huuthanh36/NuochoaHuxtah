using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using NuochoaHuxtah.Models;
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


//builder.Services.AddIdentity<AppUserModel,IdentityRole>() //(options => options.SignIn.RequireConfirmedAccount = true) Xác thực tài khoản
//	.AddEntityFrameworkStores<DbContext>().AddDefaultTokenProviders();
//builder.Services.AddRazorPages();

//builder.Services.Configure<IdentityOptions>(options =>
//{
//	// Password settings. custom password
//	options.Password.RequireDigit = true; // Yêu cầu số
//	options.Password.RequireLowercase = true; // Yêu cầu chữ thường
//	options.Password.RequireNonAlphanumeric = false; // Kí tự đặc biệt
//	options.Password.RequireUppercase = true; // Chữ hoa
//	options.Password.RequiredLength = 6; // Chiều dài password
//	/*options.Password.RequiredUniqueChars = 1;*/ // Yêu cầu 1 kí tự đặc biệt

//	//// Lockout settings.
//	//options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa acc sau 5 phút
//	//options.Lockout.MaxFailedAccessAttempts = 5; // Đăng nhập thất bại tối đa 5 lần
//	//options.Lockout.AllowedForNewUsers = true; // Cho phép user mới

//	//// User settings.
//	//options.User.AllowedUserNameCharacters =
//	//"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // Yêu cầu các kí tự có trog chuỗi
//	options.User.RequireUniqueEmail = true; // Yêu cầu email
//});


builder.Services.AddIdentity<AppUserModel,IdentityRole>()
	.AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
	// Password settings.
	options.Password.RequireDigit = true; 
	options.Password.RequireLowercase = true;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
	options.Password.RequiredLength = 4;
	options.User.RequireUniqueEmail = false;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();


app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

app.UseSession();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles(); 

app.UseRouting();   

app.UseAuthentication();
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
