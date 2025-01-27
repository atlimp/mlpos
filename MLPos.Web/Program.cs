using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using MLPos.Core.Interfaces.Repositories;
using MLPos.Core.Interfaces.Services;
using MLPos.Core.Model;
using MLPos.Data.Postgres;
using MLPos.Services;
using MLPos.Web.Middleware;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MLPos.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddEnvironmentVariables("MLPOS_");

        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
        }

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

        builder.Services.AddCors();

        builder.Services.AddHttpContextAccessor();

        builder.Logging.AddLog4Net();


        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        builder.Services.AddMvc()
            .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix);

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                "en",
                "is",
            };

            options.SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
        });

        var app = builder.Build();

        app.UseCors(builder => builder
            .AllowAnyHeader()
            .AllowAnyOrigin()
            .AllowAnyMethod());

        app.UseRequestLocalization();

        app.Use(async (ctx, next) =>
        {
            await next();

            if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
            {
                //Re-execute the request so the user gets the error page
                string originalPath = ctx.Request.Path.Value;
                ctx.Items["originalPath"] = originalPath;
                ctx.Request.Path = "/error/404";
                await next();
            }
        });

        app.UseMiddleware<ApiExceptionMiddleware>();
        app.UseMiddleware<LoggingMiddleware>();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error/500");
        }

        app.UseHttpsRedirection();
        app.UseDefaultFiles();
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
        services.AddScoped<IProductRepository, MLPos.Data.Postgres.ProductRepository>(r => new ProductRepository(connectionString));
        services.AddScoped<ICustomerRepository, MLPos.Data.Postgres.CustomerRepository>(r => new CustomerRepository(connectionString));
        services.AddScoped<IPaymentMethodRepository, MLPos.Data.Postgres.PaymentMethodRepository>(r => new PaymentMethodRepository(connectionString));
        services.AddScoped<IPosClientRepository, MLPos.Data.Postgres.PosClientRepository>(r => new PosClientRepository(connectionString));
        services.AddScoped<ITransactionHeaderRepository, MLPos.Data.Postgres.TransactionHeaderRepository>(r => new TransactionHeaderRepository(connectionString));
        services.AddScoped<ITransactionLineRepository, MLPos.Data.Postgres.TransactionLineRepository>(r => new TransactionLineRepository(connectionString));
        services.AddScoped<IPostedTransactionHeaderRepository, MLPos.Data.Postgres.PostedTransactionHeaderRepository>(r => new PostedTransactionHeaderRepository(connectionString));
        services.AddScoped<IPostedTransactionLineRepository, MLPos.Data.Postgres.PostedTransactionLineRepository>(r => new PostedTransactionLineRepository(connectionString));
        services.AddScoped<IUserRepository, MLPos.Data.Postgres.UserRepository>(r => new UserRepository(connectionString));
        services.AddScoped<IInventoryRepository, MLPos.Data.Postgres.InventoryRepository>(r => new InventoryRepository(connectionString));
        services.AddScoped<IInvoiceHeaderRepository, MLPos.Data.Postgres.InvoiceHeaderRepository>(r => new InvoiceHeaderRepository(connectionString));
        services.AddScoped<IInvoiceLineRepository, MLPos.Data.Postgres.InvoiceLineRepository>(r => new InvoiceLineRepository(connectionString));
        services.AddScoped<IDbContext, MLPos.Data.Postgres.DbContext>(r => new DbContext(connectionString));
    }

    private static void InitServices(IServiceCollection services)
    {
        services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddTransient<IProductService, ProductService>();
        services.AddTransient<ICustomerService, CustomerService>();
        services.AddTransient<IPaymentMethodService, PaymentMethodService>();
        services.AddTransient<IPosClientService, PosClientService>();
        services.AddTransient<IImageService, ImageService>();
        services.AddTransient<ITransactionService, TransactionService>();
        services.AddTransient<ILoginService, LoginService>();
        services.AddTransient<ITransactionPostingService, TransactionPostingService>();
        services.AddTransient<IInvoicingService, InvoicingService>();
    }
}