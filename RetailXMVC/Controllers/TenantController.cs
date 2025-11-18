using Microsoft.AspNetCore.Mvc;

namespace RetailXMVC.Controllers
{
    public class TenantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignUpTenant()
        {
            return View();
        }
    }
}
