using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;

        public OrdersController(IOrderRepository orderRepo, IProductRepository productRepo)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
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
        public IActionResult Create()
        {
            ViewBag.Products = _productRepo.GetAll();
            return View();
        }

        public IActionResult Edit(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null) return NotFound();

            ViewBag.Products = _productRepo.GetAll();
            return View(order);
        }

        // =============================
        // POST /Orders/Create
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Order order, int[] productIds, int[] quantities)
        {
            if (productIds.Length != quantities.Length)
            {
                ModelState.AddModelError("", "Sản phẩm và số lượng không khớp!");
                ViewBag.Products = _productRepo.GetAll();
                return View(order);
            }
            order.CreateDate = DateTime.Now;
            order.Status = 1;

            order.OrderDetails = new List<OrderDetail>();

            for (int i = 0; i < productIds.Length; i++)
            {
                var product = _productRepo.GetById(productIds[i]);

                if (product == null)
                {
                    ModelState.AddModelError("", "Sản phẩm không tồn tại!");
                    ViewBag.Products = _productRepo.GetAll();
                    return View(order);
                }

                if (quantities[i] > product.Quantity)
                {
                    ModelState.AddModelError("", 
                        $"Sản phẩm {product.ProductName} không đủ hàng. (Còn: {product.Quantity})");

                    ViewBag.Products = _productRepo.GetAll();
                    return View(order);
                }

                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = productIds[i],
                    Quantity = quantities[i]
                });

                product.Quantity -= quantities[i];
                _productRepo.Update(product);
            }

            _orderRepo.Insert(order);

            return RedirectToAction(nameof(Index));
        }


     //   [HttpPost]
     //   public IActionResult Edit(
     //int OrderId,
     //int CustomerId,
     //int StaffId,
     //int[] productIds,
     //int[] quantities,
     //bool[] selected)
     //   {
     //       var order = _orderRepo.GetById(OrderId);
     //       if (order == null) return NotFound();

     //       order.CustomerId = CustomerId;
     //       order.StaffId = StaffId;

     //       List<OrderDetail> newDetails = new List<OrderDetail>();

     //       for (int i = 0; i < productIds.Length; i++)
     //       {
     //           if (selected[i] && quantities[i] > 0)
     //           {
     //               newDetails.Add(new OrderDetail
     //               {
     //                   ProductId = productIds[i],
     //                   Quantity = quantities[i]
     //               });
     //           }
     //       }

     //       _orderRepo.Update(order, newDetails);

     //       return RedirectToAction("Index");
     //   }






        // =============================
        // GET /Orders/Delete/5
        // =============================
        public IActionResult Delete(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null) return NotFound();

            return RedirectToAction(nameof(Index));
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
