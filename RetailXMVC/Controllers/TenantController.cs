using BusinessObjectRetailX.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Repositories;
using RepositoriesRetailX;
using System.Security.Claims;

namespace RetailXMVC.Controllers
{
    public class TenantController : Controller
    {
        private readonly IUserRepository userRepo;
        private readonly ITenantRepository tenantRepo;
        public TenantController(IUserRepository userRepo, ITenantRepository tenantRepo)
        {
            this.userRepo = userRepo;
            this.tenantRepo = tenantRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignUpTenant()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUpTenant(string companyName)
        {
            var user = userRepo.GetUserByEmail(User.Identity.Name);
            if (user.TenantId != null) return RedirectToAction("LoginTenant");

            Tenant tempTenant = new Tenant
            {
                CompanyName = companyName,
                OwnerName = user.FullName,
                OwnerEmail = user.Email,
                DbServer = "localhost",
                DbName = "RetailX_Tenant_" + Guid.NewGuid().ToString("N"),
                DbUser = "sa",
                DbPassword = "12345",
                IsActive = true,
                CreatedDate = DateTime.Now
            };
            try
            {
                tenantRepo.AddTenant(tempTenant);
                CreateDatabaseForTenant(tempTenant);
                user.TenantId = tempTenant.Id;
                user.GlobalRole = "Owner";
                userRepo.UpdateUser(user);
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("FullName", user.FullName ?? user.Email),
            new Claim("GlobalRole", user.GlobalRole ?? "User"),
            new Claim("TenantId", user.TenantId?.ToString() ?? "")
        };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal
                );
            } catch (Exception)
            {
                Console.WriteLine("Lỗi ở Add Tenant");
            }
            
            return RedirectToAction("LoginTenant");
        }

        [HttpGet]
        public IActionResult LoginTenant()
        {
            return View();
        }
        private string BuildTenantConnectionString(Tenant tenant)
        {
            return $"Server=.;Database={tenant.DbName};Trusted_Connection=True;TrustServerCertificate=True;";
        }
        private void CreateDatabaseForTenant(Tenant tenant)
        {
            CreateDatabaseIfNotExists(tenant.DbName);
            ExecuteSqlScriptOnTenant(tenant, "RetailX_Schema.sql");
        }

        private void CreateDatabaseIfNotExists(string dbName)
        {
            const string masterConnStr =
                "Server=.;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

            using var conn = new SqlConnection(masterConnStr);
            conn.Open();

            var sql = $@"
IF DB_ID('{dbName}') IS NULL
BEGIN
    CREATE DATABASE [{dbName}];
END";

            using var cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }
        private void ExecuteSqlScriptOnTenant(Tenant tenant, string scriptPath)
        {
            var connStr = BuildTenantConnectionString(tenant);
            using var conn = new SqlConnection(connStr);
            conn.Open();

            var script = System.IO.File.ReadAllText(scriptPath);

            using var cmd = new SqlCommand(script, conn);
            cmd.ExecuteNonQuery();
        }
    }
}
