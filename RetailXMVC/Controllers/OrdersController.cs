using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly ICategoryRepository _categoryRepo;
        public OrdersController(IOrderRepository orderRepo, IProductRepository productRepo, ICustomerRepository customerRepo,ICategoryRepository categoryRepo)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _customerRepo = customerRepo;
            _categoryRepo = categoryRepo;
        }

        // =============================
        // GET /Orders
        // =============================
        public IActionResult Index()
        {
            var orders = _orderRepo.GetAll();
            return View(orders);
        }

        // =============================
        // GET /Orders/Details/5
        // =============================
        public IActionResult Details(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null) return NotFound();

            return View(order);
        }

        // =============================
        // GET /Orders/Create
        // =============================
        public IActionResult Create(int? categoryId, string phone = null)
        {
            // Lấy danh sách category
            ViewBag.Categories = _categoryRepo.GetAll().Where(c => c.IsActive).ToList();
            ViewBag.SelectedCategory = categoryId;

            // Lấy danh sách product theo category
            if (categoryId.HasValue && categoryId > 0)
                ViewBag.Products = _productRepo.GetAll()
                                    .Where(p => p.CategoryId == categoryId && p.IsActive)
                                    .ToList();
            else
                ViewBag.Products = _productRepo.GetAll();

            // Xử lý tìm khách hàng nếu có phone
            if (!string.IsNullOrWhiteSpace(phone))
            {
                ViewBag.Phone = phone;
                var cus = _customerRepo.GetByPhone(phone);

                if (cus != null)
                {
                    ViewBag.FoundCustomer = cus;
                }
            }

            return View(new Order());
        }



        //public IActionResult Edit(int id)
        //{
        //    var order = _orderRepo.GetById(id);
        //    if (order == null) return NotFound();

        //    ViewBag.Products = _productRepo.GetAll();
        //    return View(order);
        //}

        // =============================
        // POST /Orders/Create
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order order, int[] productIds, int[] quantities)
        {
            try
            {
                // ---------------- CHECK INPUT ----------------
                if (order.CustomerId <= 0)
                {
                    ModelState.AddModelError("", "Bạn chưa chọn khách hàng.");
                    ViewBag.Products = _productRepo.GetAll();
                    return View(order);
                }

                if (productIds == null || quantities == null || productIds.Length == 0)
                {
                    ModelState.AddModelError("", "Bạn chưa chọn sản phẩm.");
                    ViewBag.Products = _productRepo.GetAll();
                    return View(order);
                }

                if (productIds.Length != quantities.Length)
                {
                    ModelState.AddModelError("", "Sản phẩm và số lượng không khớp.");
                    ViewBag.Products = _productRepo.GetAll();
                    return View(order);
                }

                // ---------------- SET ORDER INFO ----------------
                order.CreateDate = DateTime.Now;
                order.Status = 1;
                order.OrderDetails = new List<OrderDetail>();

                // ---------------- PROCESS PRODUCTS ----------------
                for (int i = 0; i < productIds.Length; i++)
                {
                    var product = _productRepo.GetById(productIds[i]);

                    if (product == null)
                    {
                        ModelState.AddModelError("", "Sản phẩm không tồn tại.");
                        ViewBag.Products = _productRepo.GetAll();
                        return View(order);
                    }

                    if (quantities[i] > product.Quantity)
                    {
                        ModelState.AddModelError("", $"Sản phẩm {product.ProductName} không đủ hàng (còn {product.Quantity}).");
                        ViewBag.Products = _productRepo.GetAll();
                        return View(order);
                    }

                    // Thêm chi tiết đơn hàng
                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = productIds[i],
                        Quantity = quantities[i]
                    });

                    // Cập nhật tồn kho
                    product.Quantity -= quantities[i];
                    _productRepo.Update(product); // ✅ dùng repository
                }

                // ---------------- INSERT ORDER ----------------
                _orderRepo.Insert(order);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Xử lý lỗi ngoại lệ
                return Content("Lỗi: " + ex.Message + "<br/>" + ex.StackTrace);
            }
        }

        [HttpPost]
        public IActionResult FindCustomer(string phone)
        {
            var cus = _customerRepo.GetByPhone(phone);

            ViewBag.Products = _productRepo.GetAll();
            ViewBag.Phone = phone;

            ViewBag.FoundCustomer = cus;  // ❗ chỉ dùng 1 biến duy nhất

            return View("Create", new Order());
        }



        [HttpPost]
        public IActionResult AddCustomerAjax([FromBody] Customer customer)
        {
            if (customer == null) return BadRequest();

            _customerRepo.Insert(customer);

            return Json(new
            {
                id = customer.CustomerId,
                name = customer.CustomerName,
                phone = customer.Phone
            });
        }
        // =============================
        // GET /Orders/Delete/5
        // =============================
        public IActionResult Delete(int id)
        {
            _orderRepo.Delete(id);
            TempData["msg"] = "Đã hủy hóa đơn.";
            return RedirectToAction("Index");
        }


        // =============================
        // POST /Orders/DeleteConfirmed
        // =============================    
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _orderRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
