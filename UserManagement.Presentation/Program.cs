using Domain.Entities.user;
using Microsoft.AspNetCore.Identity;
using UserManagement.Persistence;
using UserManagement.Persistence.context;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPersistenceServices();
builder.Services.AddInfrastructureServices();

var cs = builder.Configuration.GetConnectionString("Dev");

//CORS
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

//Serilog
Logger log = new LoggerConfiguration()
    .WriteTo.File("logs/log.txt")
    .Enrich.FromLogContext()// bu kod ile log contexte middleware ile sonradan eklediðimiz propertylerdeki deðerlere ulaþýyoruz.
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog(log);

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;

});


builder.Services.AddControllers();

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// gizli bilgileri tutmak amaçlý
builder.Configuration.AddUserSecrets<Program>();
builder.Configuration.AddJsonFile("secrets.json", optional: true);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true,// hangi sitelerin kullanacðýný belirlediðimiz alan www.bilmemne
            ValidateIssuer = true,  // oluþturulacak token ý kimin dapýttýðýný belirlediðimiz alan API kýsmý myapi.com
            ValidateLifetime = true, // token süresini kontrol ettiðimiz yer
            ValidateIssuerSigningKey = true, // üretilecek token deðerinin uygulamamýza ait olduðunu belirlediðimiz alan security key verisinin doðrulanmasý

            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            // üretilen tokenýn expire süresini ayarlýyoruz
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

            NameClaimType = ClaimTypes.Name // jwt üzerinde Name claimne karþýlýk gelen deðeri User.Identity.Name properrtysnden elde edebiliriz.
        };

    }

)
     .AddJwtBearer("ordinaryUser", options =>
     {
         options.TokenValidationParameters = new()
         {
             ValidateAudience = true,
             ValidateIssuer = true,  // oluþturulacak token ý kimin dapýttýðýný belirlediðimiz alan API kýsmý myapi.com
             ValidateLifetime = true, // token süresini kontrol ettiðimiz yer
             ValidateIssuerSigningKey = true, // üretilecek token deðerinin uygulamamýza ait olduðunu belirlediðimiz alan security key verisinin doðrulanmasý

             ValidAudience = builder.Configuration["userToken:Audience"],
             ValidIssuer = builder.Configuration["userToken:Issuer"],
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["userToken:SecurityKey"])),
             // üretilen tokenýn expire süresini ayarlýyoruz
             LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

             NameClaimType = ClaimTypes.Name, // jwt üzerinde Name claimne karþýlýk gelen deðeri User.Identity.Name properrtysnden elde edebiliriz.

         };

     }
     );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//bu middleware kendinden sonrakileri loglatýr öncekiler loglatmaz
app.UseSerilogRequestLogging();
app.UseHttpLogging();

app.UseDefaultFiles();
app.UseStaticFiles();


app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
