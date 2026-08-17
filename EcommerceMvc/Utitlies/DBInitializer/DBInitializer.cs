using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceMvc.Utitlies.DBInitializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DBInitializer> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DBInitializer(ApplicationDbContext context, ILogger<DBInitializer> logger, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public void Initialize()
        {
            try
            {

                if (_context.Database.GetPendingMigrations().Any())
                     _context.Database.Migrate();

                if (_roleManager.Roles.IsNullOrEmpty()) {

                    // the best way when making seeding is to use GetAwaiter().GetResult() instead of using async await because it will block the thread until the task is completed and it will not cause any deadlock issues.so for that im sure the app not start without creating the roles in the database.
                    _roleManager.CreateAsync(new(SataticData.ADMIN_ROLE)).GetAwaiter().GetResult();
                     _roleManager.CreateAsync(new(SataticData.EMPLOYEE_ROLE)).GetAwaiter().GetResult();
                     _roleManager.CreateAsync(new(SataticData.CUSTOMER_ROLE)).GetAwaiter().GetResult();
                     _roleManager.CreateAsync(new(SataticData.SUPER_ADMIN_ROLE)).GetAwaiter().GetResult();

                 var result=   _userManager.CreateAsync(new() { 
                    
                        Email="superadmin@example.com",
                        UserName = "superadmin",
                        FirstName = "Super",
                        LastName = "Admin",
                        EmailConfirmed = true,
                    },"SuperAdmin@123").GetAwaiter().GetResult() ;
                    var user=_userManager.FindByNameAsync("superadmin").GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(user!, SataticData.SUPER_ADMIN_ROLE);
                }

                
            }
            catch (Exception ex) { 
                _logger.LogError($"Error : {ex.Message}");
            }
        }
    }
}
