using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IStaffRepository _staffRepo;
        public OrdersController(IOrderRepository orderRepo, IProductRepository productRepo, ICustomerRepository customerRepo, ICategoryRepository categoryRepo, IStaffRepository staffRepo  )
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _customerRepo = customerRepo;
            _categoryRepo = categoryRepo;
            _staffRepo = staffRepo;
        }


        [HttpGet]
        public IActionResult Create()
        {
            var userEmail = User.Identity?.Name; 

            int currentStaffId = 0;
            string currentStaffName = "Không xác định";

            if (!string.IsNullOrEmpty(userEmail))
            {
             
                var allStaff = _staffRepo.GetStaffListForOwner();

              
                var staff = allStaff.FirstOrDefault(s => s.Email == userEmail);

                if (staff != null)
                {
                    currentStaffId = staff.StaffId;
                    currentStaffName = staff.StaffName;
                }
            }

            ViewBag.CurrentStaffId = currentStaffId;
            ViewBag.CurrentStaffName = currentStaffName;
           

            PrepareViewData(null, null);
            return View(new Order());
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order order, int[] productIds, int[] quantities, string submitAction, int? filterCategoryId, string phone)
        {


          
       
            var (currentCart, totalAmount) = BuildCart(productIds, quantities, submitAction);

           
            if (submitAction == "create")
            {
                var userEmail = User.Identity?.Name;
                var allStaff = _staffRepo.GetStaffListForOwner();
                var staff = allStaff.FirstOrDefault(s => s.Email == userEmail);

                if (staff != null) order.StaffId = staff.StaffId;
                else order.StaffId = 1; 

                if (TryCreateOrder(order, currentCart))
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            
            ViewBag.Cart = currentCart;
            ViewBag.TotalAmount = totalAmount;
            var reloadEmail = User.Identity?.Name;
            var reloadStaff = _staffRepo.GetStaffListForOwner().FirstOrDefault(s => s.Email == reloadEmail);
            if (reloadStaff != null)
            {
                ViewBag.CurrentStaffId = reloadStaff.StaffId;
                ViewBag.CurrentStaffName = reloadStaff.StaffName;
            }
            else
            {
                ViewBag.CurrentStaffId = 1;
                ViewBag.CurrentStaffName = "Unknown";
            }
            if (!string.IsNullOrWhiteSpace(phone))
            {
                var cus = _customerRepo.GetByPhone(phone);
                if (cus != null) ViewBag.FoundCustomer = cus;
            }

            PrepareViewData(filterCategoryId, phone);
            return View(order);
        }

 
        private (List<OrderDetail>, decimal) BuildCart(int[] ids, int[] qtys, string action)
        {
            var mergedCart = new Dictionary<int, int>();
            decimal total = 0;
            var resultList = new List<OrderDetail>();

            if (ids == null || qtys == null) return (resultList, 0);

  
            for (int i = 0; i < ids.Length; i++)
            {
                if (qtys[i] > 0)
                {
                    if (mergedCart.ContainsKey(ids[i])) mergedCart[ids[i]] += qtys[i];
                    else mergedCart[ids[i]] = qtys[i];
                }
            }

 
            if (!string.IsNullOrEmpty(action) && action.StartsWith("remove-"))
            {
                if (int.TryParse(action.Split('-')[1], out int idToRemove))
                {
                    mergedCart.Remove(idToRemove);
                }
            }

   
            foreach (var item in mergedCart)
            {
                var product = _productRepo.GetById(item.Key);
                if (product != null)
                {
                    resultList.Add(new OrderDetail { ProductId = item.Key, Quantity = item.Value, Product = product });
                    total += (product.Price ?? 0) * item.Value;
                }
            }

            return (resultList, total);
        }

       
        private bool TryCreateOrder(Order order, List<OrderDetail> cart)
        {
            if (order.CustomerId <= 0) ModelState.AddModelError("", "Chưa chọn khách hàng.");
            if (cart.Count == 0) ModelState.AddModelError("", "Giỏ hàng trống.");

            // Check tồn kho
            foreach (var item in cart)
            {
                if (item.Product.Quantity < item.Quantity)
                    ModelState.AddModelError("", $"Sản phẩm {item.Product.ProductName} không đủ hàng (Còn {item.Product.Quantity}).");
            }

            if (!ModelState.IsValid) return false;

    
            order.CreateDate = DateTime.Now;
            order.Status = 1;
            order.OrderDetails = cart;

            // Trừ kho
            foreach (var item in cart)
            {
                var p = _productRepo.GetById(item.ProductId);
                p.Quantity -= item.Quantity;
                _productRepo.Update(p);
            }

            _orderRepo.Insert(order);
            return true;
        }

      
        private void PrepareViewData(int? categoryId, string phone)
        {
            ViewBag.Categories = _categoryRepo.GetAll().Where(c => c.IsActive).ToList();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.Phone = phone;

            if (categoryId.HasValue && categoryId > 0)
                ViewBag.Products = _productRepo.GetAll().Where(p => p.CategoryId == categoryId && p.IsActive).ToList();
            else
                ViewBag.Products = _productRepo.GetAll();
        }


        [HttpPost]
        public IActionResult AddCustomerAjax([FromBody] Customer cus)
        {
            // 1. Validate cơ bản
            if (cus == null || string.IsNullOrWhiteSpace(cus.Phone))
            {
                return BadRequest(new { success = false, message = "Tên và Số điện thoại là bắt buộc." });
            }

            // 2. KIỂM TRA TRÙNG SỐ ĐIỆN THOẠI (Bắt buộc)
            var existingCus = _customerRepo.GetByPhone(cus.Phone);
            if (existingCus != null)
            {
                return Json(new { success = false, message = $"SĐT {cus.Phone} đã tồn tại ({existingCus.CustomerName})!" });
            }

            // 3. KIỂM TRA TRÙNG EMAIL (Tùy chọn - Chỉ check nếu có nhập)
            if (!string.IsNullOrWhiteSpace(cus.Email))
            {
                // Giả sử Repo của bạn có hàm GetByEmail. 
                // Nếu chưa có, bạn cần vào ICustomerRepository và CustomerDAO thêm vào nhé.
                var existingEmail = _customerRepo.GetByEmail(cus.Email);

                if (existingEmail != null)
                {
                    return Json(new { success = false, message = $"Email {cus.Email} đã được sử dụng bởi khách hàng: {existingEmail.CustomerName}!" });
                }
            }

            // 4. THÊM MỚI
            try
            {
                cus.IsActive = true;
                _customerRepo.Insert(cus);

                return Json(new
                {
                    success = true,
                    id = cus.CustomerId,
                    name = cus.CustomerName,
                    phone = cus.Phone
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        public IActionResult Index(string keyword, DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            int pageSize = 10; // 10 đơn mỗi trang

            
            var (orders, totalCount) = _orderRepo.GetOrders(keyword, fromDate, toDate, page, pageSize);

            
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

          
            ViewBag.Keyword = keyword;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd"); 
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            return View(orders);
        }
        public IActionResult Details(int id) => View(_orderRepo.GetById(id));
        public IActionResult Delete(int id) { _orderRepo.Delete(id); return RedirectToAction("Index"); }
    }
}