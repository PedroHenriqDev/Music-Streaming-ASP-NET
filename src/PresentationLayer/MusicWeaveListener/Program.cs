using ApplicationLayer.Factories;
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

builder.Services.AddScoped<RecordService>();
builder.Services.AddScoped<DeleteService>();
builder.Services.AddScoped<LoginService<Listener>>();
builder.Services.AddScoped<VerifyService>();
builder.Services.AddScoped<GenerateIntelliTextService>();
builder.Services.AddScoped<CloudStorageService>();
builder.Services.AddScoped<PictureService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<UserAuthenticationService>();
builder.Services.AddScoped<UpdateService>();
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
