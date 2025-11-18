using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoriesRetailX;

namespace RetailXMVC.Controllers
{
    [Authorize(Roles = "Owner")]
    public class StaffController : Controller
    {
        private readonly IStaffRepository _staffRepo;
        private readonly IUserRepository _userRepo;
        private readonly ITenantRepository _tenantRepo;

        // 1. Inject cả 2 luồng Repository
        public StaffController(IStaffRepository staffRepo,
                               IUserRepository userRepo,
                               ITenantRepository tenantRepo)
        {
            _staffRepo = staffRepo;
            _userRepo = userRepo;
            _tenantRepo = tenantRepo;
        }

        // 2. Lấy danh sách staff từ DB Riêng
        public async Task<IActionResult> Index()
        {
            // _staffRepo đã tự động kết nối đến ĐÚNG database của Owner
            var staffList = await _staffRepo.GetStaffListForOwner();
            return View(staffList);
        }

        // 3. Ví dụ nghiệp vụ "Duyệt yêu cầu gia nhập"
        // (Dùng cả 2 luồng DB)
        [HttpPost]
        public async Task<IActionResult> ApproveRequest(int userId)
        {
            // Lấy TenantId của Owner hiện tại (đang đăng nhập)
            int currentOwnerTenantId = ...; // (Lấy từ User.Claims)

            // --- (Luồng Master) ---
            // Lấy user từ DB Chung
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null || user.TenantId != null)
            {
                return BadRequest("User không hợp lệ.");
            }

            // Gán TenantId cho user này
            user.TenantId = currentOwnerTenantId;
            await _userRepo.UpdateUserAsync(user);

            // --- (Luồng Tenant) ---
            // Tạo một record Staff mới trong DB Riêng của Owner
            var newStaff = new Staff
            {
                StaffID = user.Id, // Quan trọng: Liên kết bằng ID
                StaffName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = 2, // 2 = Role "Staff" (ví dụ)
                IsActive = true
                // ... các trường khác
            };

            await _staffRepo.CreateStaffAsync(newStaff);

            return RedirectToAction(nameof(Index));
        }
    }
}
