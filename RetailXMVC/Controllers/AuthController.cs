using BusinessObjectRetailX.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoriesRetailX;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RetailXMVC.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
      
        private readonly IUserRepository _userRepository;
        private readonly ISystemLogRepository systemLogRepository;
        public AuthController(IUserRepository userRepository, ISystemLogRepository systemLogRepository)
        {
            _userRepository = userRepository;
            this.systemLogRepository = systemLogRepository;
        }
        [HttpGet]
        [AllowAnonymous]

        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Login(string email, string password)
        {
            Console.WriteLine($"Attempting login for email: {email} : {password}");

            var fieldErrors = new Dictionary<string, string>();

            // ✅ Validate email
            if (string.IsNullOrWhiteSpace(email))
            {
                fieldErrors["Email"] = "Email không được để trống.";
            }
            else
            {
                var emailAttr = new EmailAddressAttribute();
                if (!emailAttr.IsValid(email))
                {
                    fieldErrors["Email"] = "Email không đúng định dạng.";
                }
            }

            // ✅ Validate password
            if (string.IsNullOrWhiteSpace(password))
            {
                fieldErrors["Password"] = "Mật khẩu không được để trống.";
            }

            // Nếu lỗi validate input → trả về view kèm lỗi
            if (fieldErrors.Any())
            {
                ViewBag.FieldErrors = fieldErrors;
                ViewBag.Email = email;
                return View();
            }
            User user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                fieldErrors["Email"] = "Tài khoản không tồn tại.";
                ViewBag.FieldErrors = fieldErrors;
                ViewBag.Email = email;
                return View();
            }
            // ✅ Check tài khoản / mật khẩu
            if (!_userRepository.VerifyUser(email, password))
            {
                // có thể gán lỗi chung cho cả 2 field
                fieldErrors["Password"] = "Mật khẩu không chính xác.";

                ViewBag.FieldErrors = fieldErrors;
                ViewBag.Email = email;
                return View();
            }

            // ✅ Lấy user + tạo claims
            

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Role, user.GlobalRole),
        new Claim("FullName", user.FullName ?? user.Email),
        new Claim("GlobalRole", user.GlobalRole),
        new Claim("TenantId", user.TenantId?.ToString() ?? ""),
        new Claim("StaffId", user.StaffId?.ToString() ?? ""),
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

            Console.WriteLine("User authenticated successfully.");

            if (user.GlobalRole == "Admin")
                return RedirectToAction("System", "AdminRetailX");

            if (user.GlobalRole == "Owner")
                return RedirectToAction("Index", "Staff");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]

        public IActionResult SignUpAccount()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]

        public IActionResult SignUpAccount(string email, string password, string fullname)
        {
            var errors = new Dictionary<string, string>();

            // Fullname
            if (string.IsNullOrWhiteSpace(fullname))
                errors["FullName"] = "Họ và tên không được để trống.";

            // Email
            if (string.IsNullOrWhiteSpace(email))
            {
                errors["Email"] = "Email không được để trống.";
            }
            else
            {
                var emailAttr = new EmailAddressAttribute();
                if (!emailAttr.IsValid(email))
                {
                    errors["Email"] = "Email không đúng định dạng.";
                }
                else if (_userRepository.GetUserByEmail(email) != null)
                {
                    errors["Email"] = "Email đã tồn tại.";
                }
            }

            // Password
            if (string.IsNullOrWhiteSpace(password))
            {
                errors["Password"] = "Mật khẩu không được để trống.";
            }
            else if (password.Length < 6)
            {
                errors["Password"] = "Mật khẩu phải ít nhất 6 ký tự.";
            }

            // Nếu có lỗi -> trả về View + giữ lại dữ liệu đã nhập
            if (errors.Any())
            {
                ViewBag.FieldErrors = errors;
                ViewBag.FullName = fullname;
                ViewBag.Email = email;
                return View();
            }

            // Gọi repo tạo user
            if (_userRepository.SignUpUser(email, password, fullname))
            {
                TempData["Message"] = "Đăng ký thành công. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login");
            }

            // Lỗi chung (VD: lỗi DB)
            ViewBag.ErrorMessage = "Đăng ký thất bại. Vui lòng thử lại.";
            ViewBag.FullName = fullname;
            ViewBag.Email = email;
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        [AllowAnonymous]

        public IActionResult AccessDenied(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        private User GetCurrentUser()
        {
            return _userRepository.GetUserByEmail(User.Identity.Name); ;

        }
        [HttpGet]
        public IActionResult Profile()
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(user);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string fullName, string phone)
        {
            var errors = new Dictionary<string, string>();
            var user = GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Validate
            if (string.IsNullOrWhiteSpace(fullName))
            {
                errors["FullName"] = "Họ và tên không được để trống.";
            }

            if (!string.IsNullOrWhiteSpace(phone) && phone.Length < 8)
            {
                errors["Phone"] = "Số điện thoại không hợp lệ.";
            }

            if (errors.Any())
            {
                ViewBag.ProfileErrors = errors;
                // fill lại dữ liệu đã nhập
                user.FullName = fullName;
                user.Phone = phone;
                return View("Profile", user);
            }

            // Update thông tin
            user.FullName = fullName;
            user.Phone = phone;

            var ok = _userRepository.UpdateUser(user);
            if (!ok)
            {
                ViewBag.ProfileErrorMessage = "Cập nhật thông tin thất bại. Vui lòng thử lại.";
            }
            else
            {

                var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Role, user.GlobalRole),
        new Claim("FullName", user.FullName ?? user.Email),
        new Claim("GlobalRole", user.GlobalRole),
        new Claim("TenantId", user.TenantId?.ToString() ?? ""),
        new Claim("StaffId", user.StaffId?.ToString() ?? ""),
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
                ViewBag.ProfileSuccessMessage = "Cập nhật thông tin thành công.";
            }

            return View("Profile", user);
        }
        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var errors = new Dictionary<string, string>();
            var user = GetCurrentUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            bool wantChangePassword =
                !string.IsNullOrWhiteSpace(currentPassword) ||
                !string.IsNullOrWhiteSpace(newPassword) ||
                !string.IsNullOrWhiteSpace(confirmPassword);

            if (!wantChangePassword)
            {
                // Không nhập gì mà submit → coi như không làm gì
                return RedirectToAction("Profile");
            }

            // Validate mật khẩu
            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                errors["CurrentPassword"] = "Vui lòng nhập mật khẩu hiện tại.";
            }
            else
            {
                // check mật khẩu cũ đúng không (dùng VerifyUser đã dùng BCrypt)
                bool okCurrent = _userRepository.VerifyUser(user.Email, currentPassword);
                if (!okCurrent)
                {
                    errors["CurrentPassword"] = "Mật khẩu hiện tại không đúng.";
                }
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                errors["NewPassword"] = "Mật khẩu mới không được để trống.";
            }
            else if (newPassword.Length < 6)
            {
                errors["NewPassword"] = "Mật khẩu mới phải ít nhất 6 ký tự.";
            }

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                errors["ConfirmPassword"] = "Vui lòng nhập lại mật khẩu mới.";
            }
            else if (newPassword != confirmPassword)
            {
                errors["ConfirmPassword"] = "Mật khẩu nhập lại không khớp.";
            }

            if (errors.Any())
            {
                ViewBag.PasswordErrors = errors;
                return View("Profile", user);
            }

            // Hash mật khẩu mới bằng BCrypt và lưu
            string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordHash = newHash;

            var updated = _userRepository.UpdateUser(user);
            if (!updated)
            {
                ViewBag.PasswordErrorMessage = "Đổi mật khẩu thất bại. Vui lòng thử lại.";
            }
            else
            {
                ViewBag.PasswordSuccessMessage = "Đổi mật khẩu thành công.";
            }

            return View("Profile", user);
        }
        [Authorize]
        public async Task<IActionResult> RefreshClaims()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _userRepository.GetUserById(userId);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Role, user.GlobalRole),
        new Claim("FullName", user.FullName ?? user.Email),
        new Claim("GlobalRole", user.GlobalRole ?? "User"),
        new Claim("TenantId", user.TenantId?.ToString() ?? ""),
        new Claim("StaffId", user.StaffId?.ToString() ?? ""),
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

    }
}
