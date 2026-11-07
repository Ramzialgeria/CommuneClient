using CommuneClient.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var keyPath = builder.Configuration["DataProtection:KeyPath"]
    ?? "App_Data/Keys";

var absoluteKeyPath = Path.IsPathRooted(keyPath)
    ? keyPath
    : Path.Combine(builder.Environment.ContentRootPath, keyPath);

Directory.CreateDirectory(absoluteKeyPath);

builder.Services
    .AddDataProtection()
    .SetApplicationName("CommuneClient.Web")
    .PersistKeysToFileSystem(new DirectoryInfo(absoluteKeyPath));

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".CommuneClient.Web.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Lax;

        // HTTP is used only on the trusted local network.
        // Set this to Always when the site is moved to HTTPS.
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        // The CommuneServer JWT expires after eight hours; do not extend
        // the browser ticket beyond that token's lifetime.
        options.SlidingExpiration = false;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

var apiBaseUrl = builder.Configuration["CommuneApi:BaseUrl"]
    ?? throw new InvalidOperationException("CommuneApi:BaseUrl is missing.");

if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var apiUri) ||
    (apiUri.Scheme != Uri.UriSchemeHttp && apiUri.Scheme != Uri.UriSchemeHttps))
{
    throw new InvalidOperationException("CommuneApi:BaseUrl must be a valid HTTP or HTTPS URL.");
}

builder.Services.AddHttpClient<CommuneApiClient>(client =>
{
    client.BaseAddress = apiUri;
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
