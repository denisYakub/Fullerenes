using Fullerenes.Server.DataBase;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Factories.Factories;
using Fullerenes.Server.Mappers;
using Fullerenes.Server.Middlewares;
using Fullerenes.Server.Services.IServices;
using Fullerenes.Server.Services.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")))
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(option =>
        option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddAuthorization();

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

builder.Services.AddScoped<SystemAbstractFactoryCreator, SystemOSIFactoryCreator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapIdentityApi<IdentityUser>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/logout", async (SignInManager<IdentityUser> manager) => 
{
    await manager.SignOutAsync();

    return Results.Ok();
}
).RequireAuthorization();

app.MapPost("/ping-auth", (ClaimsPrincipal user) =>
{
    var email = user.FindFirstValue(ClaimTypes.Email);

    return Results.Json(new { Email = email });
}
).RequireAuthorization();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
