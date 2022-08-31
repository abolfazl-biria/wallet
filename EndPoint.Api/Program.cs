using Application.Interfaces.Contexts;
using Application.Services.Wallets.Commands.AddWallet;
using Application.Services.Wallets.Commands.BlockingInventory;
using Application.Services.Wallets.Commands.DeleteWallet;
using Application.Services.Wallets.Commands.DepositWallet;
using Application.Services.Wallets.Commands.EditUserNameWallet;
using Application.Services.Wallets.Commands.TransferWallet;
using Application.Services.Wallets.Commands.WithdrawWallet;
using Application.Services.Wallets.Queries.GetAllWallets;
using Application.Services.Wallets.Queries.GetWalletById;
using Application.Services.Wallets.Queries.GetWalletInventory;
using Domain.Entities;
using FluentValidation.AspNetCore;
using Infrastructure.CustomeIdentity;
using Infrastructure.MappingProfile;
using Infrastructure.SeedData;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using Persistance.Contexts;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.

services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "wallet", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

services.AddSwaggerGen();

services.AddFluentValidation(options =>
{
    options.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
    options.AutomaticValidationEnabled = false;
});

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();

#region DataBaseContext

services.AddDbContext<DataBaseContext>(options =>
    options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

services.AddScoped<IDataBaseContext, DataBaseContext>();

//services.AddScoped<IDataBaseContext, DataBaseContext>();

//services.AddEntityFrameworkSqlServer()
//    .AddDbContext<DataBaseContext>(option =>
//        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

#endregion

services.AddMediatR(
    //Commands
    typeof(AddWalletCommand).Assembly,
    typeof(BlockingInventoryCommand).Assembly,
    typeof(DeleteWalletCommand).Assembly,
    typeof(DepositWalletCommand).Assembly,
    typeof(TransferWalletCommand).Assembly,
    typeof(WithdrawWalletCommand).Assembly,
    typeof(EditUserNameWalletCommand).Assembly,

    //Queries
    typeof(GetWalletByIdRequest).Assembly,
    typeof(GetAllWalletsRequest).Assembly,
    typeof(GetWalletInventoryRequest).Assembly);

services.AddAutoMapper(typeof(ProfileMapping));

services.AddIdentity<MyUser, MyRole>(options =>
{
    options.User.RequireUniqueEmail = false;

    options.Password.RequiredUniqueChars = 0;

    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Tokens.PasswordResetTokenProvider = ResetPasswordTokenProvider.ProviderKey;

    //Lokout Setting
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMilliseconds(10);

    //SignIn Setting
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<DataBaseContext>()
    .AddErrorDescriber<PersianIdentityErrorDescriber>()
    .AddDefaultTokenProviders()
    .AddTokenProvider<ResetPasswordTokenProvider>(ResetPasswordTokenProvider.ProviderKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var userManager = services.BuildServiceProvider().GetService<UserManager<MyUser>>();
var roleManager = services.BuildServiceProvider().GetService<RoleManager<MyRole>>();

await SeedData.SeedDefault(userManager, roleManager);

app.MapControllers();

app.Run();
