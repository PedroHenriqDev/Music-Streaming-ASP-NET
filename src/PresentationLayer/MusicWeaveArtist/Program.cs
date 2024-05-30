using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.Factories;
using ApplicationLayer.Services;
using DataAccessLayer.Cloud;
using DataAccessLayer.Sql;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using UtilitiesLayer.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddLogging();

builder.Services.AddScoped<RecordService>();
builder.Services.AddScoped<DeleteService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<VerifyService>();
builder.Services.AddScoped<CloudStorageService>();
builder.Services.AddScoped<GenerateIntelliTextService>();
builder.Services.AddScoped<PictureService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<UserAuthenticationService>();
builder.Services.AddScoped<UpdateService>();
builder.Services.AddScoped<UserServicesFacade<Artist>>();
builder.Services.AddScoped<ArtistFactoriesFacade>();
builder.Services.AddScoped<UserFactoriesFacade<Artist>>();
builder.Services.AddScoped<MusicServicesFacade<Artist>>();
builder.Services.AddScoped<ModelFactory>();
builder.Services.AddScoped<ViewModelFactory>();
builder.Services.AddScoped<ConnectionDb>();
builder.Services.AddScoped<ConnectionGoogleCloud>();

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
    app.UseExceptionHandler("/Main/Error");
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
    pattern: "{controller=Main}/{action=Index}/{id?}");

app.Run();