using ECommerce.Utitlies;
using EcommerceMvc.Utitlies.DBInitializer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace EcommerceMvc
{
    public static class AppConfiguration
    {
        public static void RegisterConfiguration(this IServiceCollection services, string connectionString, IConfiguration configuration)
        {


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                //3 ways to get conniction string from appsettings.json
                //options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"]);
                //options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
                options.UseSqlServer(connectionString);
            });
            services.AddIdentity<ApplicationUser, IdentityRole>(
                option =>
                {
                    option.User.RequireUniqueEmail = true;
                    option.Password.RequiredLength = 6;
                    option.Password.RequireNonAlphanumeric = false;
                    option.Lockout.MaxFailedAccessAttempts = 3;
                    option.SignIn.RequireConfirmedEmail = true;
                }
                )
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login"; // Default login path
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Default access denied path
            });

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<Brand>, Repository<Brand>>();
            services.AddScoped<IRepository<Product>, Repository<Product>>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IRepository<ProductSubImg>, Repository<ProductSubImg>>();
            services.AddScoped<IRepository<ProductColor>, Repository<ProductColor>>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRepository<ApplicationUserOtp>, Repository<ApplicationUserOtp>>();

            //external login with google
            services.AddAuthentication()
           .AddGoogle("google", opt =>
           {
               var googleAuth =
                   configuration.GetSection("Authentication:Google");

               opt.ClientId = googleAuth["ClientId"]
                   ?? throw new InvalidOperationException("Google ClientId not found.");

               opt.ClientSecret = googleAuth["ClientSecret"]
                   ?? throw new InvalidOperationException("Google ClientSecret not found.");

               opt.SignInScheme = IdentityConstants.ExternalScheme;

               opt.Events.OnRedirectToAuthorizationEndpoint = context =>
               {
                   var redirectUri = context.RedirectUri;

                   redirectUri += redirectUri.Contains('?')
                       ? "&prompt=select_account"
                       : "?prompt=select_account";

                   context.Response.Redirect(redirectUri);

                   return Task.CompletedTask;
               };
           });
            //external login with facebook
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = configuration["Authentication:Facebook:AppSecret"];
            });

            //roles and super admin seeding
            services.AddTransient<IDBInitializer, DBInitializer>();
        }
    }
}
