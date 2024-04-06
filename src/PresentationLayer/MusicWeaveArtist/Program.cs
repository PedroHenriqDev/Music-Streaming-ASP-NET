using ApplicationLayer.Facades.HelpersFacade;
using ApplicationLayer.Factories;
using ApplicationLayer.Services;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.Facades.FactoriesFacade;
using DomainLayer.Entities;
using DataAccessLayer.Cloud;
using DataAccessLayer.Sql;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using UtilitiesLayer.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddLogging();

builder.Services.AddScoped<RecordService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<VerifyService>();
builder.Services.AddScoped<GenerateIntelliTextService>();
builder.Services.AddScoped<EncryptService>();
builder.Services.AddScoped<PictureService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<MusicService>();
builder.Services.AddScoped<GoogleCloudService>();
builder.Services.AddScoped<UserAuthenticationService>();
builder.Services.AddScoped<UserPageService>();
builder.Services.AddScoped<UpdateService>();
builder.Services.AddScoped<UserServicesFacade<Artist>>();
builder.Services.AddScoped<UserHelpersFacade<Artist>>();
builder.Services.AddScoped<UserFactoriesFacade<Artist>>();
builder.Services.AddScoped<ModelFactory>();
builder.Services.AddScoped<ViewModelFactory>();
builder.Services.AddScoped<ConnectionDb>();
builder.Services.AddScoped<HttpHelper>();
builder.Services.AddScoped<ByteConvertHelper>();
builder.Services.AddScoped<JsonSerializationHelper>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddRazorPages();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Social.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 3145722800;
});

var app = builder.Build();
var configuration = new ConfigurationBuilder().SetBasePath(builder.Environment.ContentRootPath).AddJsonFile("appsettings.json").Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

string profilePicturesDirectory = Path.Combine(app.Environment.ContentRootPath, "Profile-Pictures");
if (!Directory.Exists(profilePicturesDirectory))
{
    Directory.CreateDirectory(profilePicturesDirectory);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(profilePicturesDirectory),
    RequestPath = "/profile-pictures"
});

app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();