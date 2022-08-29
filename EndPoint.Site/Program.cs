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
using Infrastructure.CustomeIdentity;
using Infrastructure.MappingProfile;
using Infrastructure.SeedData;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using Persistance.Contexts;
using System;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    var services = builder.Services;

    // Add services to the container.
    services.AddControllersWithViews();

    #region DataBaseContext

    services.AddDbContext<DataBaseContext>(options =>
        options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

    services.AddScoped<IDataBaseContext, DataBaseContext>();



    //services.AddEntityFrameworkSqlServer()
    //    .AddDbContext<DataBaseContext>(option =>
    //        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    #endregion

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

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

    services.ConfigureApplicationCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(10);
        options.SlidingExpiration = true;
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    var userManager = services.BuildServiceProvider().GetService<UserManager<MyUser>>();
    var roleManager = services.BuildServiceProvider().GetService<RoleManager<MyRole>>();

    await SeedData.SeedDefault(userManager, roleManager);

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    // NLog: catch setup errors
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}