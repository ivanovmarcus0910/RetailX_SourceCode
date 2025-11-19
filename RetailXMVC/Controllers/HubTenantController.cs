using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using RepositoriesRetailX;

namespace RetailXMVC.Controllers
{
    [Authorize(Policy = "TenantLoggedIn")]
    public class HubTenantController : Controller
    {
        private readonly IStaffRepository _staffRepo;
        private readonly IUserRepository _userRepo;
        public HubTenantController(IStaffRepository staffRepo, IUserRepository userRepo)
        {
            _staffRepo = staffRepo;
            _userRepo = userRepo;
        }
        public IActionResult Index()
        {
            var tenantIdStr = User.FindFirst("TenantId")?.Value;
            var user = _userRepo.GetUserByEmail(User.Identity.Name);

            // Không có TenantId -> bắt đi đăng ký tenant
            if (string.IsNullOrEmpty(tenantIdStr) || user.StaffId==null)
            {
                // Có thể cho message nhẹ
                TempData["TenantMessage"] = "Bạn chưa tạo Tenant. Vui lòng đăng ký công ty trước.";
                return RedirectToAction("SignUpTenant", "Tenant");
            }
            
           

            return View();
        }
    }
}
