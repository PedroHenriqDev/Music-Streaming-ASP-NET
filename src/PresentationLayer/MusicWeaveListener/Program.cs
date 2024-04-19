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
builder.Services.AddScoped<CloudStorageService>();
builder.Services.AddScoped<PictureService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<UserAuthenticationService>();
builder.Services.AddScoped<UpdateService>();
builder.Services.AddScoped<UserServicesFacade<Listener>>();
builder.Services.AddScoped<UserHelpersFacade<Listener>>();
builder.Services.AddScoped<ListenerFactoriesFacade>();
builder.Services.AddScoped<ArtistFactoriesFacade>();
builder.Services.AddScoped<UserFactoriesFacade<Listener>>();
builder.Services.AddScoped<HomeFactoriesFacades>();
builder.Services.AddScoped<ModelFactory>();
builder.Services.AddScoped<ViewModelFactory>();
builder.Services.AddScoped<ConnectionDb>();
builder.Services.AddScoped<ConnectionGoogleCloud>();
builder.Services.AddScoped<HttpHelper>();
builder.Services.AddScoped<ByteConvertHelper>();
builder.Services.AddScoped<JsonSerializationHelper>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Main/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}");

app.Run();
