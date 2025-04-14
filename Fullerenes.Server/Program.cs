using Fullerenes.Server.DataBase;
using Fullerenes.Server.Factories.AbstractFactories;
using Fullerenes.Server.Factories.Factories;
using Fullerenes.Server.Mappers;
using Fullerenes.Server.Middlewares;
using Fullerenes.Server.Objects.Fullerenes;
using Fullerenes.Server.Services.IServices;
using Fullerenes.Server.Services.Services;
using Microsoft.AspNetCore.Authentication.Negotiate;
//using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    //.AddEntityFrameworkStores<ApplicationDbContext>()
    //.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(option =>
        option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<IDataBaseService, DataBaseService>();
builder.Services.AddScoped<ICreateService, CreateService>();
builder.Services.AddScoped<IFactoryService, FactoryService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<OctreeAbstractFactory<Fullerene>, OctreeForFullerenesFactory<Fullerene>>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
