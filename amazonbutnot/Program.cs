using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using amazonbutnot.Data;
using amazonbutnot.Models;
using Microsoft.ML.OnnxRuntime;
using amazonbutnot.Middleware;

var builder = WebApplication.CreateBuilder(args);

string modelPath = "fraud_model_final.onnx";

builder.Services.AddSingleton<InferenceSession>(sp =>
{
    var session = new InferenceSession(modelPath);
    return session;
});

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("ProductDbConnection") ??
                       throw new InvalidOperationException("Connection string 'ProductDbConnection' not found for identity stuff.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Product DB setup
var productConnectionString = builder.Configuration.GetConnectionString("ProductDbConnection") ??
                       throw new InvalidOperationException("Connection string 'ProductDbConnection' not found.");
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(productConnectionString));

builder.Services.AddDefaultIdentity<Customer>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        
       
        
        options.Password.RequiredLength = 9; // Minimum password length (this is what matters baby)
        
        
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

//set up for google OAUTH
 builder.Services.AddAuthentication().AddGoogle(googleOptions =>
 {
     googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ;
googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
 });

// adding products here
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
// adding roles here
builder.Services.AddScoped<IRolesRepository, EfRolesRepository>();

// Session configuration
builder.Services.AddSession(); // Add session services

builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(4);
    options.LoginPath = "/Account/Login"; // Set the login path
    options.LogoutPath = "/Account/Logout"; // Set the logout path
    options.SlidingExpiration = true; // Resets the expiration time on each request if the user is active
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true; // Requires consent for non-essential cookies
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

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
app.UseCookiePolicy();

app.UseRouting();

//Our extra security feature
app.UseMiddleware<RateLimitMiddleware>();

app.UseAuthorization();

//CSP header stuff
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; " +
        "style-src 'self' 'unsafe-inline' https://stackpath.bootstrapcdn.com; " + // Allow inline styles and stylesheets from Bootstrap CDN
        "script-src 'self' https://code.jquery.com https://stackpath.bootstrapcdn.com; " + // Allow scripts from jQuery CDN and Bootstrap CDN
        "font-src 'self' https://stackpath.bootstrapcdn.com; " + // Allow fonts from Bootstrap CDN
        "img-src 'self' data: https:;"); // Allow images from data URLs and HTTPS sources
    await next();
});

// Add session middleware
app.UseSession();

// Map controllers and Razor pages
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=Admin}/{action=AdminIndex}"
);
app.MapRazorPages();

app.Run();
