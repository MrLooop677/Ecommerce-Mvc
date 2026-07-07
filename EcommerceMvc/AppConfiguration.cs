using Microsoft.EntityFrameworkCore;

namespace EcommerceMvc
{
    public static class AppConfiguration
    {
        public static void RegisterConfiguration(this IServiceCollection services, string connectionString)
        {
          

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                //3 ways to get conniction string from appsettings.json
                //options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"]);
                //options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<Brand>, Repository<Brand>>();
            services.AddScoped<IRepository<Product>, Repository<Product>>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IRepository<ProductSubImg>, Repository<ProductSubImg>>();
            services.AddScoped<IRepository<ProductColor>, Repository<ProductColor>>();
        }
    }
}
