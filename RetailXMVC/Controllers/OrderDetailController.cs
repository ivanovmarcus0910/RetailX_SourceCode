using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly IOrderDetailRepository _orderDetailRepo;

        public OrderDetailController(
            IOrderRepository orderRepo,
            IProductRepository productRepo,
            IOrderDetailRepository orderDetailRepo)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _orderDetailRepo = orderDetailRepo;
        }
        public IActionResult Create(int orderId)
        {
            // Lấy order để xác định nó có tồn tại không
            var order = _orderRepo.GetById(orderId);
            if (order == null) return NotFound();

            // Lấy list sản phẩm
            ViewBag.Products = _productRepo.GetAll();

            // Tạo model “trống” nhưng có OrderId
            var detail = new OrderDetail
            {
                OrderId = orderId,
                Quantity = 1
            };

            return View(detail);
        }
        // ------------------------
        //    XEM CHI TIẾT ORDER
        // ------------------------
        public IActionResult Detail(int id)
        {
            var order = _orderRepo.GetById(id);

            if (order == null)
                return NotFound();

            return View(order);  // OrderDetail/Detail.cshtml
        }


        // ------------------------
        //    FORM SỬA MỘT DÒNG
        // ------------------------
        public IActionResult Edit(int id)
        {
            var detail = _orderDetailRepo.GetById(id);
            if (detail == null) return NotFound();

            // Gửi list sản phẩm để chọn lại Product nếu muốn
            ViewBag.Products = _productRepo.GetAll();

            return View(detail);   // View: OrderDetail/Edit.cshtml
        }
        public IActionResult Delete(int id)
        {
            var detail = _orderDetailRepo.GetById(id);
            if (detail == null) return NotFound();

            int orderId = detail.OrderId;

            _orderDetailRepo.Delete(id);

            return RedirectToAction("Detail", "OrderDetail", new { id = orderId });
        }



        // ------------------------
        //    LƯU KẾT QUẢ SỬA
        // ------------------------
        [HttpPost]
        public IActionResult Edit(int id, OrderDetail detail)
        {
            if (id != detail.OrderDetailId)
                return NotFound();

            var oldDetail = _orderDetailRepo.GetById(id);
            if (oldDetail == null)
                return NotFound();

            // Lấy Product cũ & mới
            var oldProduct = _productRepo.GetById(oldDetail.ProductId);
            var newProduct = _productRepo.GetById(detail.ProductId);

            if (oldProduct == null || newProduct == null)
                return BadRequest();

            // ==============================
            // 1️⃣ Hoàn kho SP cũ (bất kể đổi hay không)
            // ==============================
            oldProduct.Quantity += oldDetail.Quantity;
            _productRepo.Update(oldProduct);

            // ==============================
            // 2️⃣ Trừ kho SP mới
            // ==============================
            if (detail.Quantity > newProduct.Quantity)
            {
                ModelState.AddModelError("",
                    $"Sản phẩm {newProduct.ProductName} không đủ hàng! (Còn: {newProduct.Quantity})");

                ViewBag.Products = _productRepo.GetAll();
                return View(detail);
            }

            newProduct.Quantity -= detail.Quantity;
            _productRepo.Update(newProduct);

            // ==============================
            // 3️⃣ Lưu OrderDetail
            // ==============================
            _orderDetailRepo.Update(detail);

            return RedirectToAction("Detail", "OrderDetail",
                new { id = detail.OrderId });
        }


        [HttpPost]
        public IActionResult Create(OrderDetail detail)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = _productRepo.GetAll();
                return View(detail);
            }

            _orderDetailRepo.Add(detail);

            return RedirectToAction("Detail", "OrderDetail", new { id = detail.OrderId });
        }

    }


}
