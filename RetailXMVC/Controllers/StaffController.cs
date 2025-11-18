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
        private readonly ILogRepository _logRepo;

        public StaffController(IStaffRepository staffRepo, IUserRepository userRepo, ILogRepository logRepo)
        {
            _staffRepo = staffRepo;
            _userRepo = userRepo;
            _logRepo = logRepo;
        }

        public IActionResult Index()
        {
            var staffList = _staffRepo.GetStaffListForOwner();
            return View(staffList);
        }

        //public IActionResult PendingRequests()
        //{
        //    var ownerTenantIdString = User.FindFirst("TenantId")?.;.Value;
        //    int currentTenantId = string.IsNullOrEmpty(ownerTenantIdString) ? 1 : int.Parse(ownerTenantIdString);
        //    var pendingUsers = _userRepo.GetUsersRequestingToJoin(currentTenantId);

        //    return View(pendingUsers);
        //}

        //[HttpPost]
        //public IActionResult ApproveStaff(int userId)
        //{
        //    try
        //    {
        //        var ownerTenantIdString = User.FindFirst("TenantId")?.Value;
        //        int currentTenantId = string.IsNullOrEmpty(ownerTenantIdString) ? 1 : int.Parse(ownerTenantIdString);

        //        var user = _userRepo.GetUserById(userId);
        //        if (user == null) return NotFound();

        //        user.TenantId = currentTenantId;
        //        user.RequestedTenantId = null;
        //        user.GlobalRole = "Staff";

        //        _userRepo.UpdateUser(user);

        //        var newStaff = new Staff
        //        {
        //            StaffId = user.Id,
        //            StaffName = user.FullName,
        //            Email = user.Email,
        //            Phone = user.Phone,
        //            Role = 2,
        //            IsActive = true,
        //            BaseSalary = 5000000
        //        };

        //        _staffRepo.CreateStaff(newStaff);
        //        _logRepo.LogCreate($"Bạn đã duyệt nhân viên {user.FullName} (ID: {user.Id})");
        //        TempData["Success"] = $"Đã duyệt nhân viên {user.FullName} thành công!";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
        //        return RedirectToAction(nameof(PendingRequests));
        //    }
        //}

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
