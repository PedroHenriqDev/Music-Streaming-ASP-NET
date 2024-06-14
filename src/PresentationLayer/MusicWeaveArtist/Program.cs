using ApplicationLayer.Factories;
using ApplicationLayer.Services;
using DataAccessLayer.Cloud;
using DataAccessLayer.Mappers;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using DataAccessLayer.Validations;
using DomainLayer.Entities;
using DomainLayer.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

async Task<UnitOfWork> CreateUnitOfWorkAsync(IServiceProvider provider, string connectionString) 
{
    var mapper = provider.GetRequiredService<DataMapper>();
    return await UnitOfWork.CreateAsync(connectionString, mapper);
}

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddLogging();

builder.Services.AddScoped<RecordService>();
builder.Services.AddScoped<DeleteService>();
builder.Services.AddScoped<LoginService<Artist>>();
builder.Services.AddScoped<VerifyService>();
builder.Services.AddScoped<CloudStorageService>();
builder.Services.AddScoped<GenerateIntelliTextService>();
builder.Services.AddScoped<PictureService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<UserAuthenticationService>();
builder.Services.AddScoped<UpdateService>();
builder.Services.AddScoped<ModelFactory>();
builder.Services.AddScoped<ViewModelFactory>();
builder.Services.AddScoped<ConnectionGoogleCloud>();
builder.Services.AddScoped<IEntitiesAssociationRepository, EntitiesAssociationRepository>();
builder.Services.AddScoped<IGenericRepository, GenericRepository>();
builder.Services.AddScoped<IMusicRepository, MusicRepository>();
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddTransient<DataMapper>();
builder.Services.AddTransient<DataValidation>();

builder.Services.AddScoped<NpgsqlConnection>(Npgsql => 
{
    return new NpgsqlConnection(connectionString);
});

builder.Services.AddSingleton<IUnitOfWork>(provider =>
{
    return CreateUnitOfWorkAsync(provider, connectionString).GetAwaiter().GetResult();
});

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