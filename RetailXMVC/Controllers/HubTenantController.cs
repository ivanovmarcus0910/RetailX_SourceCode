using Microsoft.AspNetCore.Mvc;

namespace RetailXMVC.Controllers
{
    public class HubTenantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
