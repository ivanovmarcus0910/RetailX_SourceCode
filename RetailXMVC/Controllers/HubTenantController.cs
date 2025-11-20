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

            if (string.IsNullOrEmpty(tenantIdStr) || user.StaffId==null)
            {
                TempData["TenantMessage"] = "Bạn chưa tạo Tenant. Vui lòng đăng ký công ty trước.";
                return RedirectToAction("SignUpTenant", "Tenant");
            }
            
            var staffId = user.StaffId;
            if (staffId == null)
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var staff = _staffRepo.GetStaffDetail(staffId.Value);
            Console.WriteLine("Role in Tenant" + staff.Role);
            switch (staff.Role)
            {
                case 1: Console.WriteLine("Owner"); return RedirectToAction("Index", "Staff"); break;
                    case 2: Console.WriteLine("Accountant"); return RedirectToAction("Index", "Salary"); break ;
                    case 3: Console.WriteLine("Seller"); return RedirectToAction("Create", "Orders"); break;
                    case 4: Console.WriteLine("Inventorier"); return RedirectToAction("Index", "InventoryDashboard"); break;
            }    

            return View();
        }
    }
}
