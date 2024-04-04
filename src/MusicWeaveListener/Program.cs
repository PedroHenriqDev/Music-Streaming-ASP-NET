using Microsoft.AspNetCore.Authentication.Cookies;
using Services;
using Datas.Cloud;
using Datas.Sql;
using Models.Interfaces;
using Utilities.Helpers;
using Microsoft.Extensions.FileProviders;
using Utilities.Factories;
using Facades.Factories;
using Facades.Helpers;
using Models.Entities;
using Facades.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddLogging();

builder.Services.AddScoped<RecordService>();
builder.Services.AddScoped<JsonSerializationHelper>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<VerifyService>();
builder.Services.AddScoped<HttpHelper>();
builder.Services.AddScoped<ByteConvertHelper>();
builder.Services.AddScoped<EncryptService>();
builder.Services.AddScoped<PictureService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<MusicService>();
builder.Services.AddScoped<GoogleCloudService>();
builder.Services.AddScoped<UserAuthenticationService>();
builder.Services.AddScoped<UserPageService>();
builder.Services.AddScoped<ConnectionDb>();
builder.Services.AddScoped<UserServicesFacade<Listener>>();
builder.Services.AddScoped<UserHelpersFacade<Listener>>();
builder.Services.AddScoped<UserFactoriesFacade<Listener>>();
builder.Services.AddScoped<ModelFactory>();
builder.Services.AddScoped<ViewModelFactory>();
builder.Services.AddScoped<UpdateService>();

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
    app.UseExceptionHandler("/Home/Error");
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
