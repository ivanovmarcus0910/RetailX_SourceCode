using BusinessObject.Models;
using BusinessObjectRetailX.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Repositories;
using RepositoriesRetailX;
using RetailXMVC.SignalR;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RetailXMVC.Controllers
{
    [Authorize]

    public class TenantController : Controller
    {
        private readonly IUserRepository userRepo;
        private readonly ITenantRepository tenantRepo;
        private readonly IRequestRepository requestRepo;
        private readonly IHubContext<NotificationHub> hubContext;
        private readonly IStaffRepository _staffRepo;
        private readonly ILoginHistoryRepository _loginHistoryRepo;

        public TenantController(IUserRepository userRepo, ITenantRepository tenantRepo, 
                                IRequestRepository requestRepo, IHubContext<NotificationHub> hubContext, IStaffRepository staffRepo, ILoginHistoryRepository loginHistoryRepository)
        {
            this.userRepo = userRepo;
            this.tenantRepo = tenantRepo;
            this.requestRepo = requestRepo;
            this.hubContext = hubContext;
            this._staffRepo = staffRepo;
            this._loginHistoryRepo = loginHistoryRepository;

        }


        [HttpGet]
        public IActionResult SignUpTenant()
        {
            var isTenantLogin = User.HasClaim("IsTenantLogin", "True");
            if (isTenantLogin)
            {
                return RedirectToAction("Index", "HubTenant");
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUpTenant(string companyName)
        {
            var isTenantLogin = User.HasClaim("IsTenantLogin", "True");
            if (isTenantLogin)
            {
                return RedirectToAction("Index", "HubTenant");
            }
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
            var isTenantLogin = User.HasClaim("IsTenantLogin", "True");
            if (isTenantLogin)
            {
                return RedirectToAction("Index", "HubTenant");
            }
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
            var ip = HttpContext.Request.Headers["X-Forwarded-For"]
             .FirstOrDefault()?.Split(',').First().Trim()
         ?? HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            AddLog(ip, userAgent, user.TenantId ?? 0);
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

        [HttpGet]
        public IActionResult ListCompany(string searchString)
        {
            var tenants = tenantRepo.GetAllTenantActive();

            if (!string.IsNullOrEmpty(searchString))
            {
                tenants = tenants.Where(t => t.CompanyName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.CurrentFilter = searchString;

            var requestedTenantIds = new List<int>();

            if (User.Identity.IsAuthenticated)
            {
                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                requestedTenantIds = requestRepo.GetRequestsByUserId(userId)
                                                .Select(r => r.TenantId)
                                                .ToList();
            }

            ViewBag.RequestedTenantIds = requestedTenantIds;
            return View(tenants);
        }

        [HttpGet]
        public IActionResult MyRequests()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Auth");

            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var myRequests = requestRepo.GetRequestsByUserId(userId);

            return View(myRequests);
        }

        [HttpPost]
        public async Task<IActionResult> RequestJoin(int tenantId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Forbid();
            int userId = int.Parse(userIdString);

            var user = userRepo.GetUserById(userId);

            if (user == null || user.TenantId != null)
            {
                TempData["Error"] = "Bạn đã là thành viên hoặc tài khoản không hợp lệ.";
                return RedirectToAction(nameof(ListCompany));
            }

            if (requestRepo.IsRequestPending(userId, tenantId))
            {
                TempData["Warning"] = "Yêu cầu của bạn đang chờ duyệt. Vui lòng kiên nhẫn.";
                return RedirectToAction(nameof(ListCompany));
            }

            int pendingCount = requestRepo.GetRequestsByUserId(userId).Count;
            if (pendingCount >= 5)
            {
                TempData["Error"] = "Bạn chỉ được phép gửi tối đa 5 yêu cầu cùng lúc.";
                return RedirectToAction(nameof(ListCompany));
            }

            try
            {
                requestRepo.CreateJoinRequest(userId, tenantId);

                await hubContext.Clients.Group($"Tenant_{tenantId}")
                                 .SendAsync("ReceiveJoinRequest", user.FullName, user.Email);

                TempData["Success"] = "Đã gửi yêu cầu gia nhập thành công. Vui lòng chờ Owner duyệt.";
                return RedirectToAction("MyRequests");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi gửi yêu cầu: " + ex.Message;
                return RedirectToAction(nameof(ListCompany));
            }
        }

        public bool AddLog(string ip, string device, int tenantId)
        {
            Console.WriteLine($"Đã vào add log : {ip} -  {device} -  {tenantId}");
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
            {
                Console.WriteLine("ID user null");
                return false;
            }
            try
            {
                int id = int.Parse(userIdString);
                UserLoginHistory temp = new UserLoginHistory
                {
                    UserId = id,
                    TenantId = tenantId,
                    LoginTime = DateTime.Now,
                    IpAddress = ip,
                    Device = device
                };

                _loginHistoryRepo.AddLoginHistory(temp);
                Console.WriteLine("Add thanh cong");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi add log "+ ex.Message);
                return false;
            }

        }
    }
}
