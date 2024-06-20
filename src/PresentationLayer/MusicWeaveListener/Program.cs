using ApplicationLayer.Factories;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Profiles;
using ApplicationLayer.Services;
using DataAccessLayer.Cloud;
using DataAccessLayer.Mappers;
using DataAccessLayer.UnitOfWork;
using DataAccessLayer.Validations;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using Npgsql;
using SharedComponents.Filters;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddLogging();

builder.Services.AddScoped<IRecordService, RecordService>();
builder.Services.AddScoped<IDeleteService, DeleteService>();
builder.Services.AddScoped<ILoginService<Listener>, LoginService<Listener>>();
builder.Services.AddScoped<IVerifyService, VerifyService>();
builder.Services.AddScoped<IGenerateIntelliTextService, GenerateIntelliTextService>();
builder.Services.AddScoped<ICloudStorageService, CloudStorageService>();
builder.Services.AddScoped<IPictureService, PictureService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IUserAuthenticationService ,UserAuthenticationService>();
builder.Services.AddScoped<IUpdateService, UpdateService>();
builder.Services.AddScoped<DomainFactory>();
builder.Services.AddScoped<ViewModelFactory>();
builder.Services.AddScoped<ConnectionGoogleCloud>();
builder.Services.AddTransient<DataMapper>();
builder.Services.AddTransient<DataValidation>();

builder.Services.AddAutoMapper(typeof(DomainMappingProfile).Assembly);

builder.Services.AddScoped<NpgsqlConnection>(Npgsql =>
{
    return new NpgsqlConnection(connectionString);
});

builder.Services.AddSingleton<IUnitOfWork>(provider =>
{
    return CreateUnitOfWorkAsync(provider, connectionString).GetAwaiter().GetResult();
});

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

builder.Logging.AddConsole();

var app = builder.Build();

async Task<UnitOfWork> CreateUnitOfWorkAsync(IServiceProvider provider, string connectionString)
{
    var mapper = provider.GetRequiredService<DataMapper>();
    return await UnitOfWork.CreateAsync(connectionString, mapper);
}

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
