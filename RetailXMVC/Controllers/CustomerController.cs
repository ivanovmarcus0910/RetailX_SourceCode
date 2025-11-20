using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IOrderRepository _orderRepo;
        public CustomerController(ICustomerRepository customerRepo, IOrderRepository orderRepo)
        {
            _customerRepo = customerRepo;
            _orderRepo = orderRepo;
        }

        // -----------------------------------------------
        // LIST
        // -----------------------------------------------
        public IActionResult Index()
        {
            var list = _customerRepo.GetAll().Where(c => c.IsActive).ToList();
            return View(list);
        }


        // -----------------------------------------------
        // GET: Customer/Create
        // -----------------------------------------------
        public IActionResult Create()
        {
            return View();

        }
        public IActionResult Edit(int id)
        {
            var customer = _customerRepo.GetById(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        public IActionResult History(int id)
        {
            Console.WriteLine("=== DEBUG CUSTOMER HISTORY ===");
            Console.WriteLine("CustomerId nhận được = " + id);

            var orders = _orderRepo.GetAll().Where(o => o.CustomerId == id).ToList();

            Console.WriteLine("Số đơn tìm thấy = " + orders.Count);

            return View(orders);
        }
        public IActionResult Delete(int id)
        {
            var customer = _customerRepo.GetById(id);
            if (customer == null) return NotFound();

            customer.IsActive = false;
            _customerRepo.Update(customer);

            TempData["msg"] = "Khách hàng đã được vô hiệu hóa!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Edit(Customer cus)
        {
            var current = _customerRepo.GetById(cus.CustomerId);
            if (current == null) return NotFound();

            // Reset ViewBag
            ViewBag.PhoneDuplicate = null;
            ViewBag.EmailDuplicate = null;

            // ========== CHECK PHONE DUPLICATE ==========
            if (!string.IsNullOrWhiteSpace(cus.Phone))
            {
                var dupPhone = _customerRepo.GetByPhone(cus.Phone);

                if (dupPhone != null && dupPhone.CustomerId != cus.CustomerId)
                {
                    ViewBag.PhoneDuplicate = dupPhone;
                    return View(cus);   // ❗ QUAN TRỌNG
                }
            }

            // ========== CHECK EMAIL DUPLICATE ==========
            if (!string.IsNullOrWhiteSpace(cus.Email))
            {
                var dupEmail = _customerRepo.GetByEmail(cus.Email);

                if (dupEmail != null && dupEmail.CustomerId != cus.CustomerId)
                {
                    ViewBag.EmailDuplicate = dupEmail;
                    return View(cus);   // ❗ QUAN TRỌNG
                }
            }

            // ========== UPDATE ==========
            current.CustomerName = cus.CustomerName;
            current.Phone = cus.Phone;
            current.Email = cus.Email;
            current.Address = cus.Address;

            _customerRepo.Update(current);

            TempData["msg"] = "Cập nhật thông tin khách hàng thành công!";
            return RedirectToAction("Index");
        }


        // -----------------------------------------------
        // POST: Customer/Create
        // Tạo khách hàng từ trang riêng
        // -----------------------------------------------
        [HttpPost]
        public IActionResult Create(Customer cus)
        {
            if (!ModelState.IsValid)
                return View(cus);
            cus.IsActive = true;
            _customerRepo.Insert(cus);

            TempData["msg"] = "Thêm khách hàng thành công!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult CreateFromOrder([FromForm] Customer cus)
        {
            if (string.IsNullOrWhiteSpace(cus.Phone))
                return Json(new { success = false, message = "Số điện thoại không hợp lệ!" });

            var byPhone = _customerRepo.GetByPhone(cus.Phone);

            // Nếu phone tồn tại ACTIVE
            if (byPhone != null && byPhone.IsActive)
            {
                return Json(new { success = false, message = "Số điện thoại đã được sử dụng!" });
            }

            // Nếu phone tồn tại nhưng INACTIVE → không khôi phục ở popup
            if (byPhone != null && !byPhone.IsActive)
            {
                return Json(new
                {
                    success = false,
                    message = "Khách hàng tồn tại nhưng đã bị vô hiệu hóa. Hãy vào hồ sơ để khôi phục."
                });
            }

            // Kiểm tra Email
            if (!string.IsNullOrWhiteSpace(cus.Email))
            {
                var byEmail = _customerRepo.GetByEmail(cus.Email);
                if (byEmail != null && byEmail.IsActive)
                {
                    return Json(new { success = false, message = "Email đã được sử dụng!" });
                }
            }

            // Tạo mới
            cus.IsActive = true;
            _customerRepo.Insert(cus);

            return Json(new
            {
                success = true,
                type = "create",
                id = cus.CustomerId,
                name = cus.CustomerName,
                phone = cus.Phone
            });
        }

        public IActionResult Profile(int id)
        {
            var cus = _customerRepo.GetById(id);
            if (cus == null) return NotFound();

            return View(cus);
        }

        public IActionResult Restore(int id)
        {
            var cus = _customerRepo.GetById(id);
            if (cus == null) return NotFound();

            cus.IsActive = true;
            _customerRepo.Update(cus);

            TempData["msg"] = "Khôi phục khách hàng thành công!";
            return RedirectToAction("Profile", new { id = id });
        }

        // -----------------------------------------------
        // AJAX: thêm khách hàng từ popup
        // URL: /Customer/AddCustomerAjax
        // -----------------------------------------------
        [HttpPost]
        public IActionResult AddCustomerAjax([FromBody] Customer cus)
        {
            if (cus == null || string.IsNullOrWhiteSpace(cus.Phone))
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }
            cus.IsActive = true;
            _customerRepo.Insert(cus);

            return Json(new
            {
                id = cus.CustomerId,
                name = cus.CustomerName,
                phone = cus.Phone
            });
        }

    }
}
