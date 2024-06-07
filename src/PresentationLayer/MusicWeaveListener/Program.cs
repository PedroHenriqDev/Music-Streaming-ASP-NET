using ApplicationLayer.Facades.FactoriesFacade;
using ApplicationLayer.Facades.ServicesFacade;
using ApplicationLayer.Factories;
using ApplicationLayer.Services;
using DataAccessLayer.Cloud;
using DataAccessLayer.Mappers;
using DataAccessLayer.Repositories;
using DataAccessLayer.Sql;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using SharedComponents.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddLogging();

builder.Services.AddScoped<RecordService>();
builder.Services.AddScoped<DeleteService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<VerifyService>();
builder.Services.AddScoped<GenerateIntelliTextService>();
builder.Services.AddScoped<CloudStorageService>();
builder.Services.AddScoped<PictureService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<UserAuthenticationService>();
builder.Services.AddScoped<UpdateService>();
builder.Services.AddScoped<UserServicesFacade<Listener>>();
builder.Services.AddScoped<ListenerFactoriesFacade>();
builder.Services.AddScoped<ArtistFactoriesFacade>();
builder.Services.AddScoped<UserFactoriesFacade<Listener>>();
builder.Services.AddScoped<MainServicesFacade<Listener>>();
builder.Services.AddScoped<MainFactoriesFacades>();
builder.Services.AddScoped<PlaylistServicesFacade>();
builder.Services.AddScoped<PlaylistFactoriesFacades>();
builder.Services.AddScoped<SearchServicesFacade>();
builder.Services.AddScoped<ModelFactory>();
builder.Services.AddScoped<ViewModelFactory>();
builder.Services.AddScoped<ConnectionDb>();
builder.Services.AddScoped<DataMapper>();
builder.Services.AddScoped<ConnectionGoogleCloud>();
builder.Services.AddScoped<MusicServicesFacade<Listener>>();
builder.Services.AddScoped<MusicFactoriesFacade>();
builder.Services.AddScoped<EntitiesAssociationRepository>();
builder.Services.AddScoped<GenericRepository>();
builder.Services.AddScoped<MusicRepository>();
builder.Services.AddScoped<PlaylistRepository>();
builder.Services.AddScoped<UserRepository>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers(options => 
{
    options.Filters.Add(typeof(GlobalExceptionFilter));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "DefaultCookies";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(360);
    options.SlidingExpiration = true;
});

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Social.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(90);
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
