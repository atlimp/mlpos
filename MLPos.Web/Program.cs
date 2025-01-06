using Microsoft.AspNetCore.Authentication.Cookies;
using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Interfaces.Services;
using MLPos.Data.Postgres;
using MLPos.Services;
using MLPos.Web.Middleware;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace MLPos.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);



        string connectionString = builder.Configuration.GetConnectionString("Postgres") ?? "";
        string DBUser = builder.Configuration["DBUser"] ?? "";
        string DBPass = builder.Configuration["DBPass"] ?? "";
        string DBHost = builder.Configuration["DBHost"] ?? "";
        string DBPort = builder.Configuration["DBPort"] ?? "";
        string DBName = builder.Configuration["DBName"] ?? "";
        
        connectionString = string.Format(connectionString, DBUser, DBPass, DBHost, DBPort, DBName);

        InitRepositories(builder.Services, connectionString);
        InitServices(builder.Services);

        // Add services to the container.
        builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "mlpos";
                options.LoginPath = "/Admin/Login";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            });

        builder.Services.AddHttpContextAccessor();

        builder.Logging.AddLog4Net();

        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        builder.Services.AddMvc()
            .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix);

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                "en-US",
                "is-IS",
                "en-US",
            };

            options.SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
        });

        var app = builder.Build();

        app.UseRequestLocalization();

        app.UseMiddleware<ApiExceptionMiddleware>();
        app.UseMiddleware<LoggingMiddleware>();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapAreaControllerRoute(
            name: "admin",
            areaName: "Admin",
            pattern: "Admin/{controller}/{action}/{id?}",
            defaults: new { controller = "Product", action = "Index" });

        app.Run();
    }

    private static void InitRepositories(IServiceCollection services, string connectionString = "")
    {
        services.AddSingleton<IProductRepository, MLPos.Data.Postgres.ProductRepository>(r => new ProductRepository(connectionString));
        services.AddSingleton<ICustomerRepository, MLPos.Data.Postgres.CustomerRepository>(r => new CustomerRepository(connectionString));
        services.AddSingleton<IPaymentMethodRepository, MLPos.Data.Postgres.PaymentMethodRepository>(r => new PaymentMethodRepository(connectionString));
        services.AddSingleton<ITransactionHeaderRepository, MLPos.Data.Postgres.TransactionHeaderRepository>(r => new TransactionHeaderRepository(connectionString));
        services.AddSingleton<ITransactionLineRepository, MLPos.Data.Postgres.TransactionLineRepository>(r => new TransactionLineRepository(connectionString));
    }

    private static void InitServices(IServiceCollection services)
    {
        services.AddTransient<IProductService, ProductService>();
        services.AddTransient<ICustomerService, CustomerService>();
        services.AddTransient<IPaymentMethodService, PaymentMethodService>();
        services.AddTransient<IImageService, ImageService>();
        services.AddTransient<ITransactionService, TransactionService>();
        services.AddTransient<ILoginService, SimpleLoginService>();
    }
}