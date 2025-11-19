using Azure.Core;
using BusinessObject.Models;
using BusinessObjectRetailX.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using RepositoriesRetailX;

namespace RetailXMVC.Controllers
{
    //[Authorize(Roles = "Owner")]
    public class StaffController : Controller
    {
        private readonly IStaffRepository _staffRepo;
        private readonly IUserRepository _userRepo;
        private readonly IRequestRepository _requestRepo;
        private readonly ILogRepository _logRepo;

        public StaffController(IStaffRepository staffRepo, IUserRepository userRepo, IRequestRepository requestRepo, ILogRepository logRepo)
        {
            _staffRepo = staffRepo;
            _userRepo = userRepo;
            _requestRepo = requestRepo;
            _logRepo = logRepo;
        }

        public IActionResult Index()
        {
            var staffList = _staffRepo.GetStaffListForOwner();
            return View(staffList);
        }

        public IActionResult PendingRequests()
        {
            // A. Lấy TenantId của Owner đang đăng nhập
            var ownerTenantIdString = User.FindFirst("TenantId")?.Value;
            int currentTenantId = string.IsNullOrEmpty(ownerTenantIdString) ? 1 : int.Parse(ownerTenantIdString);

            // B. Lấy danh sách Request thay vì lọc từ bảng User
            // Lấy List<Request> đã có thông tin User.
            List<BusinessObjectRetailX.Models.Request > pendingRequests = _requestRepo.GetTenantPendingRequests(currentTenantId);

            // Ở View (PendingRequests.cshtml), bạn duyệt List<Request> này 
            // và hiển thị Request.User.FullName và Request.Id để duyệt.

            return View(pendingRequests);
        }

        [HttpPost]
        public IActionResult ApproveStaff(int requestId)
        {
            if (requestId <= 0)
            {
                TempData["Error"] = "Mã yêu cầu không hợp lệ.";
                return RedirectToAction(nameof(PendingRequests));
            }
            try
            {
                var request = _requestRepo.GetRequestById(requestId);
                if (request == null) return NotFound();

                var user = _userRepo.GetUserById(request.UserId);
                if (user == null || user.TenantId != null)
                {
                    TempData["Error"] = "User không tồn tại hoặc đã là thành viên.";
                    return RedirectToAction(nameof(PendingRequests));
                }

                user.TenantId = request.TenantId;
                _userRepo.UpdateUser(user);
                //_requestRepo.UpdateRequestStatus(requestId, true);

                var newStaff = new Staff
                {
                    StaffId = user.Id,
                    StaffName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = 2,
                    IsActive = true,
                    BaseSalary = 5000000
                };

                _staffRepo.CreateStaff(newStaff);
                _logRepo.LogCreate($"Bạn đã duyệt nhân viên {user.FullName} (ID: {user.Id})", 0);

                TempData["Success"] = $"Đã duyệt nhân viên {user.FullName} thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(PendingRequests));
            }
        }

        public IActionResult Delete(int userId)
        {
            // Logic
            // 1. Xóa (hoặc set Active=false) ở DB Riêng
            // 2. Set TenantId = null ở DB Chung (để họ free)
            _logRepo.LogWarning($"Bạn đã xóa nhân viên ID: {userId}", 0);
            return View();
        }
    }
}
