using Azure.Core;
using BusinessObject.Models;
using BusinessObjectRetailX.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet]
        public IActionResult PendingRequests()
        {
            var ownerTenantIdString = User.FindFirst("TenantId")?.Value;
            int currentTenantId = string.IsNullOrEmpty(ownerTenantIdString) ? 1 : int.Parse(ownerTenantIdString);

            List<BusinessObjectRetailX.Models.Request > pendingRequests = _requestRepo.GetTenantPendingRequests(currentTenantId);

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
                _requestRepo.DeleteRequest(requestId);

                var newStaff = new Staff
                {
                    StaffName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone ?? "Not available",
                    Role = 2,
                    IsActive = true,
                    BaseSalary = 5000000
                };

                _staffRepo.CreateStaff(newStaff);
                _logRepo.LogCreate($"Bạn đã duyệt nhân viên {user.FullName} (ID: {newStaff.StaffId})", 1);

                TempData["Success"] = $"Đã duyệt nhân viên {user.FullName} thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(PendingRequests));
            }
        }

        [HttpGet]
        public IActionResult GetEditPartial(int id)
        {
            var staff = _staffRepo.GetStaffDetail(id);
            if (staff == null) return NotFound();

            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "2", Text = "Accountant" },
                new SelectListItem { Value = "3", Text = "Seller" },
                new SelectListItem { Value = "4", Text = "Inventory Manager" }
            };

            return PartialView("_EditStaffPartial", staff);
        }

        [HttpPost]
        public IActionResult Edit(Staff model)
        {
            if (model.Role == 1)
            {
                TempData["Error"] = "Bạn không thể cấp quyền Owner cho nhân viên.";
                return RedirectToAction("Index");
            }

            var staff = _staffRepo.GetStaffDetail(model.StaffId);
            if (staff != null)
            {
                staff.Role = model.Role;
                staff.BaseSalary = model.BaseSalary;
                _staffRepo.UpdateStaff(staff);
                _logRepo.LogUpdate($"Bạn đã cập nhật thông tin nhân viên {staff.StaffName} (ID: {staff.StaffId})", 1);
                TempData["Success"] = $"Cập nhật thông tin {staff.StaffName} thành công!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy nhân viên này.";
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            try
            {
                var staff = _staffRepo.GetStaffDetail(id);
                if (staff == null)
                {
                    TempData["Error"] = "Nhân viên không tồn tại.";
                    return RedirectToAction("Index");
                }

                staff.IsActive = false;
                _staffRepo.UpdateStaff(staff);

                var user = _userRepo.GetAll().FirstOrDefault(u => u.Email == staff.Email);
                if (user != null)
                {
                    user.TenantId = null;
                    _userRepo.UpdateUser(user);
                }

                _logRepo.LogWarning($"Bạn đã xóa nhân viên {staff.StaffName} (ID: {id})", 1);
                TempData["Success"] = "Đã xóa nhân viên thành công";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
