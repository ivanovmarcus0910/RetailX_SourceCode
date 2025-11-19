using BusinessObjectRetailX.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RepositoriesRetailX;
using System.Security.Claims;
using System.Security.Claims;
using System.Security.Principal;
namespace RetailXMVC.Controllers
{
    public class AuthController : Controller
    {
      
        private readonly IUserRepository _userRepository;
        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            Console.WriteLine($"Attempting login for email: {email} : {password}");
            if (!_userRepository.VerifyUser(email, password))
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }
            User user = _userRepository.GetUserByEmail(email);

            // 🔹 Tạo claims
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
            if (user.GlobalRole=="Admin")
                return RedirectToAction("System", "AdminRetailX");

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult SignUpAccount()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUpAccount(string email, string password, string fullname)
        {
            Console.WriteLine($"Sign up with {email} {password} {fullname} ");
            if (_userRepository.GetUserByEmail(email) != null)
            {
                ViewBag.ErrorMessage = "Email already exists.";
                return View();
            }
            if (_userRepository.SignUpUser(email, password, fullname))
            {
                TempData["Message"] = "Sign up successful. Please re-login.";
                return RedirectToAction("Login");
            }
            else
                            {
                ViewBag.ErrorMessage = "Sign up failed. Please try again.";
            }
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult AccessDenied(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}
