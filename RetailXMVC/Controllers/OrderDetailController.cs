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
            
            var order = _orderRepo.GetById(orderId);
            if (order == null) return NotFound();

      
            ViewBag.Products = _productRepo.GetAll();

            // Tạo model “trống” nhưng có OrderId
            var detail = new OrderDetail
            {
                OrderId = orderId,
                Quantity = 1
            };

            return View(detail);
        }
        
        public IActionResult Detail(int id)
        {
            var order = _orderRepo.GetById(id);

            if (order == null)
                return NotFound();

            return View(order);  
        }


      
        public IActionResult Edit(int id)
        {
            var detail = _orderDetailRepo.GetById(id);
            if (detail == null) return NotFound();

           
            ViewBag.Products = _productRepo.GetAll();

            return View(detail);  
        }
        public IActionResult Delete(int id)
        {
            var detail = _orderDetailRepo.GetById(id);
            if (detail == null) return NotFound();

            int orderId = detail.OrderId;
            int quantityToRestore = detail.Quantity;
            int productId = detail.ProductId;

            var product = _productRepo.GetById(productId);

            if (product == null)
            {
                _orderDetailRepo.Delete(id);
                return RedirectToAction("Detail", new { id = orderId });
            }

            product.Quantity += quantityToRestore;
            _productRepo.Update(product);

            _orderDetailRepo.Delete(id);

            return RedirectToAction("Detail", "OrderDetail", new { id = orderId });
        }



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

            oldProduct.Quantity += oldDetail.Quantity;
            _productRepo.Update(oldProduct);

            if (detail.Quantity > newProduct.Quantity)
            {
                ModelState.AddModelError("",
                    $"Sản phẩm {newProduct.ProductName} không đủ hàng! (Còn: {newProduct.Quantity})");

                ViewBag.Products = _productRepo.GetAll();
                return View(detail);
            }

            newProduct.Quantity -= detail.Quantity;
            _productRepo.Update(newProduct);

          
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
