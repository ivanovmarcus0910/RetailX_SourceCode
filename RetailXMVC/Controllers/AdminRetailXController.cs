using BusinessObjectRetailX.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoriesRetailX;
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
        public AdminRetailXController(ITenantRepository tenantRepo, IStatisticRepository statisticRepo, IUserRepository userRepo)
        {
            this.tenantRepo = tenantRepo;
            this.statisticRepo = statisticRepo;
            this.userRepo = userRepo;
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
                tenantRepo.UpdateTenant(tenant);
            }

            // Quay lại đúng trang trước (giữ search/filter)
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(TenantManager));
        }
    }
}
