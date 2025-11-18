using BusinessObject.Models;
using BusinessObjectRetailX.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Repositories;
using RepositoriesRetailX;
using System.Security.Claims;

namespace RetailXMVC.Controllers
{
    [Authorize]

    public class TenantController : Controller
    {
        private readonly IUserRepository userRepo;
        private readonly ITenantRepository tenantRepo;
        private readonly IStaffRepository _staffRepo;

        public TenantController(IUserRepository userRepo, ITenantRepository tenantRepo, IStaffRepository staffRepo)
        {
            this.userRepo = userRepo;
            this.tenantRepo = tenantRepo;
            this._staffRepo = staffRepo;
        }

        [HttpGet]
        public IActionResult SignUpTenant()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
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


                Staff tempStaff = new Staff
                {
                    StaffName = user.FullName,
                    Role = 1,
                    Phone = user.Phone??"Not available",
                    Email = user.Email,
                    Address = "Not available",
                    BaseSalary = 0,
                    IsActive = true
                };
                _staffRepo.CreateStaff(tempStaff);
                user.StaffId = tempStaff.StaffId;
                userRepo.UpdateUser(user);

            }
            catch (Exception)
            {
                Console.WriteLine("Lỗi ở Add Tenant");
            }
            
            return RedirectToAction("LoginTenant");
        }


        [HttpGet]
        public async Task<IActionResult>  LoginTenant()
        {
            var user = userRepo.GetUserByEmail(User.Identity.Name);
            var staff = _staffRepo.GetStaffDetail(user.StaffId.Value);

            if (user.TenantId == null || staff == null)
            {
                TempData["Message"] = "Bạn chưa có tenant. Vui lòng đăng ký tenant trước.";
                return RedirectToAction("SignUpTenant");
            }
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("FullName", user.FullName ?? user.Email),
            new Claim("GlobalRole", user.GlobalRole ?? "User"),
            new Claim("TenantId", user.TenantId?.ToString() ?? ""),
            new Claim("StaffId", user.StaffId?.ToString() ?? ""),
            new Claim("IsTenantLogin", "True"),
            new Claim("StaffRole", staff.Role.ToString())
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
            return RedirectToAction("Index", "HubTenant");
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
