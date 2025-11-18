using Microsoft.AspNetCore.Mvc;
using RepositoriesRetailX;

namespace RetailXMVC.Controllers
{
    public class AuthController : Controller
    {
      
        private readonly IUserRepository _userRepository;
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            
            if (!_userRepository.VerifyUser(email, password))
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View();
            }

            Console.WriteLine("User authenticated successfully.");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
    }
}
