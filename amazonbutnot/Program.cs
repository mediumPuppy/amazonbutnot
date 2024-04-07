using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using amazonbutnot.Data;
using Microsoft.AspNetCore.Http;
using amazonbutnot.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Product DB setup
var productConnectionString = builder.Configuration.GetConnectionString("ProductDbConnection") ??
                       throw new InvalidOperationException("Connection string 'ProductDbConnection' not found.");
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlite(productConnectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IProductRepository, EFProductRepository>();

// Session configuration
builder.Services.AddSession(); // Add session services

builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Add session middleware
app.UseSession();

// Map controllers and Razor pages
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
