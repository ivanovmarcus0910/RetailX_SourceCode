using BusinessObjectRetailX.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoriesRetailX;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

public class SystemStatisticViewModel
{
    public Statistic Today { get; set; }              // thống kê hôm nay
    public List<Statistic> RecentDays { get; set; } = new();  // list các ngày trước (kể cả hôm nay cũng được)
}
namespace RetailXMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminRetailXController : Controller
    {
        private readonly ITenantRepository tenantRepo;
        private readonly IStatisticRepository statisticRepo;
        private readonly IUserRepository userRepo;
        private readonly ISystemLogRepository systemLogRepo;
        private readonly ILoginHistoryRepository loginHistoryRepo;
        public AdminRetailXController(ITenantRepository tenantRepo, IStatisticRepository statisticRepo, IUserRepository userRepo, ISystemLogRepository systemLogRepo, ILoginHistoryRepository loginHistoryRepo)
        {
            this.tenantRepo = tenantRepo;
            this.statisticRepo = statisticRepo;
            this.userRepo = userRepo;
            this.systemLogRepo = systemLogRepo;
            this.loginHistoryRepo = loginHistoryRepo;
        }
        [HttpGet]
        public IActionResult System()
        {
            string day = DateTime.Now.ToString("yyyy-MM-dd");
            if (!statisticRepo.CheckStatisticExists(day))
            {
                statisticRepo.EnsureStatisticForToday();
            }
            var statToDay = statisticRepo.GetStatistic(day);
            var listStat = statisticRepo.GetStatisticList();
            var model = new SystemStatisticViewModel
            {
                Today = statToDay,
                RecentDays = listStat
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult UserManager(string? search, string? roleFilter, bool? activeFilter, int? tenantId)
        {
            var users = userRepo.GetAll();
            if (search != null)
            {
                users = users
            .Where(u =>
                (!string.IsNullOrEmpty(u.FullName) && u.FullName.ToLower().Contains(search.ToLower())) ||
                (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(search.ToLower())) ||
                (!string.IsNullOrEmpty(u.Phone) && u.Phone.ToLower().Contains(search.ToLower()))
            )
            .ToList();
            }
            if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "All")
            {
                users = users
                    .Where(u => u.GlobalRole == roleFilter)
                    .ToList();
            }
            if (activeFilter.HasValue)
            {
                users = users
                    .Where(u => u.IsActive == activeFilter.Value)
                    .ToList();
            }
            if (tenantId.HasValue)
            {
                users = users
                    .Where(u => u.TenantId == tenantId.Value)
                    .ToList();
            }
            return View(users);
        }

        [HttpPost]
        public IActionResult ToggleUserActive(int id, string? returnUrl)
        {
            var user = userRepo.GetUserById(id);
            if (user != null)
            {
                user.IsActive = !(user.IsActive ?? true);
                AddSystemLog("Change", $"Change Status of User have UserID = {user.Id} to {user.IsActive}");

                userRepo.UpdateUser(user);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(UserManager));
        }

        [HttpGet]
        public IActionResult TenantManager(string? search, bool? activeFilter)
        {
            // Lấy tất cả tenant
            var tenants = tenantRepo.GetAllTenant(); // List<Tenant>

            // Search theo tên công ty / owner / email
            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLower();

                tenants = tenants
                    .Where(t =>
                        (!string.IsNullOrEmpty(t.CompanyName) && t.CompanyName.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(t.OwnerName) && t.OwnerName.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(t.OwnerEmail) && t.OwnerEmail.ToLower().Contains(keyword)))
                    .ToList();
            }

            // Filter trạng thái (Active / Inactive)
            if (activeFilter.HasValue)
            {
                tenants = tenants
                    .Where(t => t.IsActive == activeFilter.Value)
                    .ToList();
            }

            // Sắp xếp: mới tạo lên trước
            tenants = tenants
                .OrderByDescending(t => t.CreatedDate)
                .ToList();

            return View(tenants);
        }

        [HttpPost]
        public IActionResult ToggleTenantActive(int id, string? returnUrl)
        {
            var tenant = tenantRepo.GetTenantById(id);
            if (tenant != null)
            {
                tenant.IsActive = !(tenant.IsActive ?? true);
                AddSystemLog("Change", $"Change Status of Tenant have TenantID = {tenant.Id} to {tenant.IsActive}");
                tenantRepo.UpdateTenant(tenant);
            }

            // Quay lại đúng trang trước (giữ search/filter)
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(TenantManager));
        }

        [HttpGet]
        public IActionResult SystemLogManager(string? search, int? userId)
        {
            // Lấy tất cả log từ repo
            var logs = systemLogRepo.GetAll(); // List<SystemLog>

            // Nếu GetAll() trả về IQueryable thì: var logs = systemLogRepo.GetAll().AsQueryable();

            // Search theo Action / Details / User
            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLower();

                logs = logs
                    .Where(l =>
                        (!string.IsNullOrEmpty(l.Action) && l.Action.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(l.Details) && l.Details.ToLower().Contains(keyword)) ||
                        (l.User != null && (
                        (!string.IsNullOrEmpty(l.User.Email) && l.User.Email.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(l.User.FullName) && l.User.FullName.ToLower().Contains(keyword))
                    ))

                    )
                    .ToList();
            }

            // Filter theo UserId
            if (userId.HasValue)
            {
                logs = logs
                    .Where(l => l.UserId == userId.Value)
                    .ToList();
            }

            // Sắp xếp mới nhất lên đầu
            logs = logs
                .OrderByDescending(l => l.CreatedDate)
                .ToList();

            return View(logs); // View model: List<SystemLog>
        }

        [HttpGet]
        public IActionResult UserLoginHistoryManager(string? search, int? userId, int? tenantId)
        {
            // Lấy tất cả history (repo nên Include User luôn nếu cần)
            var logs = loginHistoryRepo.GetLoginHistories(); // List<UserLoginHistory>

            // Search chung (email / fullname / ip / device)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.Trim().ToLower();

                logs = logs
                    .Where(l =>
                        (!string.IsNullOrEmpty(l.IpAddress) && l.IpAddress.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrEmpty(l.Device) && l.Device.ToLower().Contains(keyword)) ||
                        (l.User != null && (
                            (!string.IsNullOrEmpty(l.User.Email) && l.User.Email.ToLower().Contains(keyword)) ||
                            (!string.IsNullOrEmpty(l.User.FullName) && l.User.FullName.ToLower().Contains(keyword))
                        ))
                    )
                    .ToList();
            }

            if (userId.HasValue)
            {
                logs = logs
                    .Where(l => l.UserId == userId.Value)
                    .ToList();
            }

            if (tenantId.HasValue)
            {
                logs = logs
                    .Where(l => l.TenantId == tenantId.Value)
                    .ToList();
            }

            logs = logs
                .OrderByDescending(l => l.LoginTime)
                .ToList();

            return View(logs);  // View model: List<UserLoginHistory>
        }

        public bool AddSystemLog(string action, string detail)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null) {
                return false;
            }
            try
            {
                int id = int.Parse(userIdString);
                SystemLog temp = new SystemLog
                {
                    UserId = id,
                    Action = action,
                    Details = detail,
                    CreatedDate = DateTime.Now
                };

                systemLogRepo.AddLog(temp);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
           
        }
    }
}
