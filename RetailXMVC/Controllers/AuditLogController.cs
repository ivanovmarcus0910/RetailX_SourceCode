using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace RetailXMVC.Controllers
{
    // [Authorize(Roles = "Owner")]
    public class AuditLogController : Controller
    {
        private readonly ILogRepository _logRepo;
        public AuditLogController(ILogRepository logRepo)
        {
            _logRepo = logRepo;
        }   

        public IActionResult Index(DateTime? fromDate, DateTime? toDate, int? logLevel)
        {
            var logs = _logRepo.GetLogsByFilter(fromDate, toDate, logLevel, 200);

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.LogLevel = logLevel;

            return View(logs);
        }
    }
}
