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
                // 1. Lưu Tenant + tạo DB cho Tenant
                tenantRepo.AddTenant(tempTenant);
                CreateDatabaseForTenant(tempTenant);

                // 2. Cập nhật user với TenantId + role Owner
                user.TenantId = tempTenant.Id;
                Console.WriteLine("Tenant ID dang set cho user: " + user.TenantId);
                user.GlobalRole = "Owner";
                userRepo.UpdateUser(user);
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.GlobalRole),
            new Claim("FullName", user.FullName ?? user.Email),
            new Claim("GlobalRole", user.GlobalRole ?? "User"),
            new Claim("TenantId", user.TenantId?.ToString() ?? ""),
        };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal
                );
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                // có thể redirect ra trang lỗi, tuỳ đại ca
            }

            return RedirectToAction("LoginTenant");
        }



        [HttpGet]
        public async Task<IActionResult> LoginTenant()
        {
            
            var user = userRepo.GetUserByEmail(User.Identity.Name);
            if ((user.TenantId == null))
            {
                TempData["Message"] = "Bạn chưa đăng ký vào Tenant. Vui lòng liên hệ quản trị viên để yêu cầu. Hoặc đăng ký Tenant mới";
                return RedirectToAction("SignUpTenant");
            }
            if (user.StaffId == null && user.GlobalRole=="Owner")
            {
                Console.WriteLine("StaffId null, tạo mới Staff cho Owner");
                Staff tempStaff = new Staff
                {
                    StaffName = user.FullName,
                    Role = 1,
                    Phone = user.Phone ?? "Not available",
                    Email = user.Email,
                    Address = "Not available",
                    BaseSalary = 0,
                    IsActive = true,
                };
                _staffRepo.CreateStaff(tempStaff);
                user.StaffId = tempStaff.StaffId;
                userRepo.UpdateUser(user);
            }
            if (user.StaffId == null)
            {
                TempData["Message"] = "Bạn không có quyền vào Tenant này. Vui lòng liên hệ quản trị viên.";
                return RedirectToAction("SignUpTenant");
            }
            var staff = _staffRepo.GetStaffDetail(user.StaffId.Value);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.GlobalRole),
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

            Console.WriteLine($"Tenant login successful. {user.TenantId} : {user.StaffId}");
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
