using System.Security.Claims;
using Fullerenes.Server.DataBase;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Factories.Factories;
using Fullerenes.Server.Mappers;
using Fullerenes.Server.Middlewares;
using Fullerenes.Server.Services.IServices;
using Fullerenes.Server.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(option =>
        option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddScoped<SystemAbstractFactoryCreator, SystemOSIFactoryCreator>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<IDataBaseService, DataBaseService>();
builder.Services.AddScoped<ICreateService, CreateService>();
builder.Services.AddScoped<IFileService>(provider =>
{
    string folderPath = Path.Combine(
        AppContext.BaseDirectory,
        "CsvResults");

    if (!Directory.Exists(folderPath))
        Directory.CreateDirectory(folderPath);

    return new FileService(folderPath);
});

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddRazorPages();

builder.Services
    .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")))
    .AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";

    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/ping-auth") &&
           context.Response.StatusCode == 200)
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});


builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseRouting();

app.MapGet("/ping-auth", (ClaimsPrincipal user) =>
{
    var email = user.FindFirstValue(ClaimTypes.Email);

    return Results.Json(new { Email = email });
}
).RequireAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
